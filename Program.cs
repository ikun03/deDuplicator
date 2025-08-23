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
                    JsonDocument jsonData = JsonDocument.Parse(fileData);
                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("File not found: " + e.Message);
                return 3;
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
        //Step 2: Create the data structures that will handle the JSON
        //object and also handle the duplicates

        //Step 3: Output the record of changes in a JSON file with 
        //values showing the new values and the old values 
        return 0;
    }
}