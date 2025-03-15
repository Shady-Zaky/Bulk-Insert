# Bulk Data Insertion Task

## Problem Statement

As part of the WakeCap product, users need the ability to bulk insert data into the system by uploading CSV or Excel files. The system must validate the data to ensure it is in the correct format, contains relevant information, and meets the system's requirements. If the data is valid, it should be successfully stored in the database. Otherwise, users should receive feedback on any errors found in the file.

## Problem Details

Users frequently need to assign workers to specific working zones on particular dates. To facilitate this, they will upload a file containing assignment details in the following format:

| Worker Code | Zone Code | Assignment Date |
| ----------- | --------- | --------------- |
| W1          | Z1        | 2025-02-01      |

This means that the worker with code `W1` is assigned to the zone with code `Z1` on `2025-02-01`.

## Technical Details and Prerequisites

The system already has the following tables:

- `worker`: Stores worker data with columns `(id, name, code)`.
- `zone`: Stores zone data with columns `(id, name, code)`.
- `worker_zone_assignment`: Stores worker assignments with columns `(worker_id, zone_id, assignment_date)`.

**To set up the necessary data, please run the `initialize_data.sql` file to create the tables with sample data.**

## Validation Requirements

The system must validate the uploaded data based on the following rules:

1. **Worker Existence**: Each worker code in the file must exist in the `worker` table.
2. **Zone Existence**: Each zone code in the file must exist in the `zone` table.
3. **Valid Date Format**: The assignment date must be in a valid date format (e.g., `YYYY-MM-DD`).
4. **Worker Code & Zone Code Length**: Both `worker_code` and `zone_code` should not exceed 10 characters.
5. **No Duplicate Assignments in the File**:
   - A worker cannot be assigned to multiple zones on the same date.
   - There should be no exact duplicate rows (i.e., same worker, same zone, same date appearing more than once).
6. **Consistency with Existing Assignments**:
   - The new data should not introduce conflicts with existing assignments in the `worker_zone_assignment` table.
   - A worker cannot be assigned to a different zone on the same date if an assignment already exists.

## Processing Logic

The final destination for saving valid data is the `worker_zone_assignment` table. The processing workflow follows these steps:

1. **Parsing the File**: The uploaded CSV file is read and parsed into a list of objects, each containing `worker_code`, `zone_code`, and `assignment_date`.

2. Validate the data against the **Validation Requirements**.

3. **Retrieving Database References**: For valid records, the corresponding `worker_id` and `zone_id` are retrieved from the `worker` and `zone` tables.

4. **Data Insertion**: Insert the `(worker_id, zone_id, assignment_date)` into the `worker_zone_assignment` table.

5. Only files without any errors should be saved in the DB

6. **Error Reporting**: If any validation checks fail, the system generates a structured error response highlighting the issues in specific rows.

## Example Data

### Valid Rows (Will Be Inserted):

| Worker Code | Zone Code | Assignment Date |
| ----------- | --------- | --------------- |
| W1          | Z1        | 2025-02-01      |
| W2          | Z2        | 2025-02-02      |

### Invalid Rows (Will Be Rejected):

| Worker Code | Zone Code | Assignment Date | Error Reason                                                                                                                                                            |
| ----------- | --------- | --------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| LongWorkerCode123 | Z1 | 2025-02-01 | Worker code exceeds 10 characters |
| W1 | VeryLongZoneCode123 | 2025-02-01 | Zone code exceeds 10 characters |
| W3          | Z1        | 2025-02-01      | Worker does not exist                                                                                                                                                   |
| W1          | Z3        | 2025-02-01      | Zone does not exist                                                                                                                                                     |
| W1          | Z1        | 2025-02-01      | Duplicate row in file                                                                                                                                                   |
| W1          | Z2        | 2025-02-01      | Worker assigned to multiple zones on the same date                                                                                                                      |
| **W1**      | **Z1**    | **2025-02-01**  | **Assignment already exists in the table** |
| **W1**      | **Z2**    | **2025-02-01**  | **If inserted, it would create a conflicting assignment for the same worker on the same date**                                                                          |

### Conflicting Data in `worker_zone_assignment` Table:

| Worker ID | Zone ID | Assignment Date |                                                                                                           |
| --------- | ------- | --------------- | --------------------------------------------------------------------------------------------------------- |
| **101**   | **201** | **2025-02-01**  | ***(Existing assignment for W1 in Z1, causing rejection for duplicate row and conflicting assignments)*** |
| **101**   | **202** | **2025-02-01**  | ***(Existing assignment for W1 in Z2, preventing assignment to a different zone on the same date)***      |

## Expected Behavior

- If all validation checks pass, the system should store the assignments in the database.
- If any validation checks fail, the system should provide clear feedback to the user, indicating which rows contain errors and why.

## Deliverables

1. The bulk data insertion service should be implemented as an **ASP.NET Core API**.
2. The API should provide an **endpoint that accepts a CSV file** as input.
3. The API should only accept files with the following schema:
   ```csv
   worker_code,zone_code,assignment_date
   ```
4. The upload endpoint should **parse the CSV file into a list of objects** to begin validation.
5. If the file is valid, the data should be **saved directly to the database** in the `worker_zone_assignment` table.
6. If the file contains invalid rows, the API should return a **detailed JSON response** indicating the errors.
7. The API should be able to process a **50,000-row file in less than 60 seconds**.
8. The API should **not accept files with more than 50,000 rows**.
9. The service should **keep track of all uploaded files' statuses(saved,rejected)**.
10. The service should include **all EF Core migration classes** for any newly created tables required to achieve the API output.
11. Please use **PostgreSQL as the database management system (DBMS)** and **EF Core as the Object-Relational Mapper (ORM)**.
12. Please attach the endpoints in postman collection 
13. please attach the csv files used for testing the API
14. **the task should be deliverd in public githup repository**
### API Response Example:

```json
[
  {
    "data": { "worker_code": "LongWorkerCode123", "zone_code": "LongZoneCode123", "assignment_date": "2025-35-01" },
    "error": { "worker_code": "Worker code exceeds 10 characters","zone_code": "Worker code exceeds 10 characters","assignment_date":"invalid date format" }
  },
  
  {
    "data": { "worker_code": "W3", "zone_code": "Z500", "assignment_date": "2025-02-01" },
    "error": { "worker_code": "Worker does not exist","Zone_code": "Zone does not exist" }
  },
  {
    "data": { "worker_code": "W1", "zone_code": "Z3", "assignment_date": "2025-02-01" },
    "error": { "zone_code": "Zone does not exist" }
  },
  {
    "data": { "worker_code": "W1", "zone_code": "Z1", "assignment_date": "2025-02-01" },
    "error": { "rowError": "Duplicate row in file" }
  },
  {
    "data": { "worker_code": "W1", "zone_code": "Z2", "assignment_date": "2025-02-01" },
    "error": { "rowError": "Worker assigned to multiple zones on the same date" }
  },
  {
    "data": { "worker_code": "W1", "zone_code": "Z1", "assignment_date": "2025-02-01" },
    "error": { "rowError": "Assignment already exists in worker_zone_assignment table" }
  },
  {
    "data": { "worker_code": "W1", "zone_code": "Z2", "assignment_date": "2025-02-01" },
    "error": { "rowError": "If inserted, it would create a conflicting assignment for the same worker on the same date" }
  }
]
```



