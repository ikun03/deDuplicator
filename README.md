# Lead Deduplicator

A C# console application that processes lead data from JSON files and removes duplicates based on ID and email conflicts, preserving the most recent entries while maintaining a history of all conflicting records.

## What it does

The Lead Deduplicator merges duplicate records by:

- **Identifying conflicts**: Finds leads with matching IDs or email addresses
- **Preserving latest data**: Keeps the most recent entry based on timestamp
- **Maintaining history**: Stores all conflicting records in a `sourceLeads` array
- **Smart conflict resolution**: Handles complex scenarios where the same ID appears with different emails or vice versa

## Features

- Processes JSON files with lead data
- Handles both direct JSON arrays and wrapper objects with "leads" property
- Preserves original date formatting
- Outputs clean, structured JSON with deduplicated leads
- Comprehensive error handling and validation

## Prerequisites

- .NET 8.0 or later
- C# compiler

## Installation

1. Clone or download this repository
2. Navigate to the project directory
3. Build the project:

```bash
dotnet build
```

## Usage

Run the application with a JSON file containing lead data:

```bash
dotnet run <input-file.json>
```

### Example

```bash
dotnet run leads.json
```

### Input Format

The application accepts JSON files in either format:

**Wrapper format:**
```json
{
  "leads": [
    {
      "_id": "unique_id_1",
      "email": "user@example.com",
      "firstName": "John",
      "lastName": "Doe",
      "address": "123 Main St",
      "entryDate": "2014-05-07T17:30:20+00:00"
    }
  ]
}
```

**Direct array format:**
```json
[
  {
    "_id": "unique_id_1",
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "address": "123 Main St",
    "entryDate": "2014-05-07T17:30:20+00:00"
  }
]
```

### Output Format

The application creates a new file with `_deDuplicated.json` suffix containing:

```json
{
  "leads": [
    {
      "_id": "winning_id",
      "email": "winning@example.com",
      "firstName": "John",
      "lastName": "Doe",
      "address": "123 Main St", 
      "entryDate": "2014-05-07T17:33:20+00:00",
      "sourceLeads": [
        {
          "_id": "conflicting_id_1",
          "email": "conflict1@example.com",
          "firstName": "John",
          "lastName": "Doe",
          "address": "456 Other St",
          "entryDate": "2014-05-07T17:32:20+00:00"
        }
      ]
    }
  ]
}
```

## How Deduplication Works

1. **Conflict Detection**: Identifies leads with matching IDs or email addresses
2. **Timestamp Comparison**: Uses `entryDate` to determine the most recent entry
3. **Winner Selection**: The lead with the latest timestamp becomes the primary record
4. **Data Consolidation**: All conflicting records are stored in the `sourceLeads` array
5. **Complex Conflicts**: When the same ID appears with different emails (or vice versa), the algorithm intelligently merges all related records

## Exit Codes

- `0`: Success
- `1`: No input file provided
- `2`: General application error
- `3`: Input file not found
- `4`: Invalid JSON format

## Example Output

For an input file with 10 leads containing duplicates, you might see:

```
Number of leads processed from the source file: 10
Deduplicated leads written to: leads_deDuplicated.json
Number of deduplicated leads: 4
```

## Error Handling

The application includes comprehensive error handling for:
- Missing or invalid input files
- Malformed JSON data
- Invalid date formats
- Missing required properties

## License

This project is open source. See LICENSE file for details.
