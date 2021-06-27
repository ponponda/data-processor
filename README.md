# Data Source Processor

The library enables frontend to retrieve results from server based on given api request.

### Properties

Property | Type | Description
-- | -- | --
Filter | ["$Field", "$operator", "$value"] | $operator: optional, "="(default), "!=", ">", "<", ">=", "<=".
Sort | { field: string, desc: boolean } |
TotalSummary | { field: string, type: string }[] | type: min, max, sum, avg, count
Group | { field: string }[] | 
GroupSummary | { field: string, type: string }[] | type: min, max, sum, avg, count
Skip | int |
Take | int |

### Response
```
    GroupResult
    {
        key: primitive type,
        data: GroupResult
        summary: number[]
    }
    
    Every result
    {
        data: any | GroupResult(If you specify Group),
        totalCount: number,
        summary: number[]
    },
```

### Examples
#### Backend ( C# )
```
public IActionResult Get(DataSourceLoadOption options) {
    var result = DataProcessor.Load('$YourDataSource', options);
    return Ok(result);
}
```

#### Frontend ( Javascript )
```
const params = new URLSearchParams();
```
```
const andCond = [
    [$field, "test"], [$field, ">" , 2]
];
params.set("filter", JSON.stringify(andCond));

const orCond = [
    [$field, "test"], "or", [$field, ">" , 2]
];
params.set("filter", JSON.stringify(orCond));

const andOrCond = [
    [$field, "<" , 90], 
    "and"
    [[$field, "test"], "or", [$field, ">" , 2]]
];
params.set("filter", JSON.stringify(andOrCond));
```
```
params.set("sort", { field: "", desc: false });
params.set("totalSummary", 
JSON.stringify([
   { field: "", type: "sum" }
]));
params.set("group", [
JSON.stringify(
   { field: "" }
));
params.set("groupSummary", 
JSON.stringify([
   { field: "", type: "sum" }
]));
params.set("skip", 0);
params.set("take", 10);

const path = `${Your domain}?${params.toString()}`;
const xhr = new XMLHttpRequest();
xhr.open('GET', path, true);
xhr.send(null);
```
