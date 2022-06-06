The entity to interact with is the msdyn_timeentry entity:

| Field Name | Schema Name | Field Type |
|------------|-------------|------------|
| Start      | Msdyn_start | DateTime   | 
| End        | Msdyn_end   | DateTime   |

The azure function accepts the payload:

```
{
  "$schema": "http://json-schema.org/draft-04/schema#",
  "type": "object",
  "properties": {
    "StartOn": {
      "type": "string",
      "format": "date"
    },
    "EndOn": {
      "type": "string",
      "format": "date"
    }
  },
  "required": [
    "StartOn",
    "EndOn"
  ]
}
```
 
The function creates an msdyn_timeentry record for every date in the date range from StartOn to EndOn. Duplicate time entry records don't create.