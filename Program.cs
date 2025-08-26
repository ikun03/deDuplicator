using System.Text.Json;

public class Program
{
    private static void MergeLeads(ref List<Leads> outputList, int existingLeadIndex, Leads inputLead, 
                                  ref Dictionary<string, int> idLeadDict, ref Dictionary<string, int> emailLeadsDict, 
                                  bool updateIdDict, bool updateEmailDict)
    {
        int comp = DateUtils.CompareDates(outputList[existingLeadIndex].EntryDate, inputLead.EntryDate);
        if (comp == -2) throw new Exception("One of the JSON dates are invalid");
        
        if (comp == 1)
        {
            outputList[existingLeadIndex].SourceLeads.Add(inputLead);
        }
        else if (comp <= 0)
        {
            outputList[existingLeadIndex].FirstName = inputLead.FirstName;
            outputList[existingLeadIndex].LastName = inputLead.LastName;
            outputList[existingLeadIndex].Address = inputLead.Address;
            outputList[existingLeadIndex].EntryDate = inputLead.EntryDate;
            if (updateEmailDict)
            { outputList[existingLeadIndex].Email = inputLead.Email; }
            else if (updateIdDict)
            {
                outputList[existingLeadIndex].Id = inputLead.Id;
            }
            outputList[existingLeadIndex].SourceLeads.Add(inputLead);
        }
        
        if (updateIdDict)
        {
            idLeadDict.Add(outputList[existingLeadIndex].Id, existingLeadIndex);
        }
        
        if (updateEmailDict)
        {
            emailLeadsDict.Add(outputList[existingLeadIndex].Email, existingLeadIndex);
        }
    }

    public static int Main(string[] args)
    {
        //Step 1: Open the input file and read it into a JSON object
        if (args.Length > 0)
        {
            try
            {
                string fileName = args[0];
                if (!File.Exists(fileName))
                {
                    throw new FileNotFoundException();
                }
                else
                {
                    string fileData = File.ReadAllText(fileName);

                    List<Leads>? inputLeads = null;
                    
                    // Try to deserialize as direct array first
                    // If that fails, try to deserialize as wrapper object
                    var wrapper = JsonSerializer.Deserialize<JsonElement>(fileData);
                    if (wrapper.TryGetProperty("leads", out JsonElement leadsElement))
                    {
                        inputLeads = JsonSerializer.Deserialize<List<Leads>>(leadsElement.GetRawText());
                        if (inputLeads == null)
                        {
                            throw new JsonException("Could not parse leads from JSON");
                        }
                        Console.WriteLine("Number of leads processed from the source file: " + inputLeads.Count);
                    }
                    else
                    {
                        throw new JsonException("Could not parse leads from JSON - 'leads' property not found");
                    }

                    //Step 2: Create the data structures that will handle the JSON
                    //object and also handle the duplicates

                    //2a: Create 2 lists to track duplicate ids and emails
                    Dictionary<string, int> idLeadDict = new();
                    Dictionary<string, int> emailLeadsDict = new();
                    List<Leads> outputList = new();

                    //2b: Now start parsing the list of leads from the input
                    foreach (var inputLead in inputLeads)
                    {
                        //2c: Check if either of the dictionary has the entry for this id or email
                        if (idLeadDict.ContainsKey(inputLead.Id) || emailLeadsDict.ContainsKey(inputLead.Email))
                        {
                            //We have detected a conflict
                            //We need to resolve it. 

                            //2d: First let's see if the idLeadDict has a conflicting entry
                            int idLeadIdx = -1;
                            if (idLeadDict.ContainsKey(inputLead.Id))
                            {
                                idLeadIdx = idLeadDict[inputLead.Id];
                            }

                            //2e: Now let's see if the emailLeadDict has a conflicting entry
                            int emailLeadIdx = -1;
                            if (emailLeadsDict.ContainsKey(inputLead.Email))
                            {
                                emailLeadIdx = emailLeadsDict[inputLead.Email];
                            }

                            if (emailLeadIdx != -1 && idLeadIdx != -1 && emailLeadIdx != idLeadIdx)
                            {
                                var emailBasedLead = outputList[emailLeadIdx];
                                var idBasedLead = outputList[idLeadIdx];

                                //2f: First we need to reconcile the two entries that we already have 
                                int comp = DateUtils.CompareDates(emailBasedLead.EntryDate, idBasedLead.EntryDate);
                                if (comp == -2)
                                {
                                    throw new Exception("One of the JSON dates are invalid");
                                }

                                bool emailBasedLeadIsWinner = false;
                                bool idBasedLeadIsWinner = false;

                                if (comp == 1)
                                {
                                    emailBasedLeadIsWinner = true;
                                }
                                else if (comp == -1)
                                {
                                    idBasedLeadIsWinner = true;
                                }
                                else if (comp == 0)
                                {
                                    if (emailLeadIdx > idLeadIdx)
                                    {
                                        emailBasedLeadIsWinner = true;
                                    }
                                    else
                                    {
                                        idBasedLeadIsWinner = true;
                                    }
                                }

                                //2g: The email based lead is greater. 
                                if (emailBasedLeadIsWinner)
                                {
                                    //The id based lead must also point here
                                    //Therefore we must first update the id based lead to be invalid
                                    idBasedLead.IsValid = false;
                                    //Then we need to merge it's sources with email based lead's sources
                                    emailBasedLead.SourceLeads.AddRange(idBasedLead.SourceLeads);
                                    //Then finally the dict is updated
                                    idLeadDict[idBasedLead.Id] = emailLeadIdx;

                                    //Now since email based lead is the POR
                                    //We will merge the input lead with that
                                    MergeLeads(ref outputList, emailLeadIdx, inputLead, ref idLeadDict, ref emailLeadsDict, false, false);

                                }
                                //2h: The id based lead is greater
                                else if (idBasedLeadIsWinner)
                                {
                                    //Same as above the email based lead must also point here
                                    //Therefore we must first update the email based lead to be invalid. 
                                    emailBasedLead.IsValid = false;
                                    //Then we need to merge it's sources with id based lead's sources
                                    idBasedLead.SourceLeads.AddRange(emailBasedLead.SourceLeads);
                                    //Then finally dict is updated
                                    emailLeadsDict[emailBasedLead.Email] = idLeadIdx;
                                    MergeLeads(ref outputList, idLeadIdx, inputLead, ref idLeadDict, ref emailLeadsDict, false, false);
                                }

                            }
                            else if (emailLeadIdx == idLeadIdx)
                            {
                                //2i: Both are pointing to the same idx node
                                MergeLeads(ref outputList, emailLeadIdx, inputLead, ref idLeadDict, ref emailLeadsDict, false, false);
                            }
                            else if (emailLeadIdx == -1)
                            {
                                MergeLeads(ref outputList, idLeadIdx, inputLead, ref idLeadDict, ref emailLeadsDict, false, true);
                            }
                            else if (idLeadIdx == -1)
                            {
                                MergeLeads(ref outputList, emailLeadIdx, inputLead, ref idLeadDict, ref emailLeadsDict, true, false);
                            }

                        }
                        else
                        {
                            //2j: If not then add the entry to the dictionary and add object to the list
                            outputList.Add(inputLead);
                            int idx = outputList.Count - 1;
                            idLeadDict.Add(inputLead.Id, idx);
                            emailLeadsDict.Add(inputLead.Email, idx);
                            if (outputList[idx].SourceLeads == null)
                            {
                                outputList[idx].SourceLeads = new List<Leads>();
                            }
                            outputList[idx].SourceLeads.Add(inputLead);
                            outputList[idx].IsValid = true;
                        }
                    }

                    //3: Filter valid leads and create output JSON
                    var validLeads = outputList.Where(lead => lead.IsValid).ToList();
                    
                    // Create simplified objects for serialization
                    var outputData = validLeads.Select(lead => new
                    {
                        _id = lead.Id,
                        email = lead.Email,
                        firstName = lead.FirstName,
                        lastName = lead.LastName,
                        address = lead.Address,
                        entryDate = lead.EntryDate,
                        sourceLeads = lead.SourceLeads?.Select(sourceLead => new
                        {
                            _id = sourceLead.Id,
                            email = sourceLead.Email,
                            firstName = sourceLead.FirstName,
                            lastName = sourceLead.LastName,
                            address = sourceLead.Address,
                            entryDate = sourceLead.EntryDate
                            // Omit sourceLeads and isValid for nested items
                        }).ToList()
                        // Omit isValid for root level items
                    }).ToList();
                    
                    //Step 4: Create JSON options
                    var options = new JsonSerializerOptions 
                    { 
                        WriteIndented = true,
                        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                    };
                    
                    var wrappedOutput = new { leads = outputData };
                    string outputJson = JsonSerializer.Serialize(wrappedOutput, options);
                    
                    //Step 5: Write to output file
                    string outputFileName = Path.GetFileNameWithoutExtension(fileName) + "_deDuplicated.json";
                    File.WriteAllText(outputFileName, outputJson);
                    
                    Console.WriteLine("Deduplicated leads written to: "+outputFileName);
                    Console.WriteLine("Number of deduplicated leads: "+validLeads.Count);



                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("File not found: " + e.Message);
                return 3;
            }
            catch (JsonException e)
            {
                Console.WriteLine("Invalid JSON Format: " + e.Message);
                return 4;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in application: " + e.Message);
                return 2;

            }
        }
        else
        {
            Console.WriteLine("No input provided. Please provide input file.");
            return 1;
        }

        //Step 3: Output the record of changes in a JSON file with 
        //values showing the new values and the old values 
        return 0;
    }
}