using System.Text.Json;
public class Program
{
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

                    List<Leads>? inputLeads = JsonSerializer.Deserialize<List<Leads>>(fileData);
                    if (inputLeads == null)
                    {
                        throw new JsonException("List of leads is null");
                    }
                    else
                    {
                        Console.WriteLine("Number of leads processed from the source file: " + inputLeads.Count);
                    }

                    //Step 2: Create the data structures that will handle the JSON
                    //object and also handle the duplicates

                    //2a: Create 2 lists to track duplicate ids and emails
                    Dictionary<string, int> idLeadDict = new();
                    Dictionary<string, int> emailLeadsDict = new();
                    List<Leads> outputList = new();

                    //Now start parsing the list of leads from the input
                    foreach (var inputLead in inputLeads)
                    {

                    }


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