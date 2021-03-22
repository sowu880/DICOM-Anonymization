# Command Line

```

./Dicom-Anonymization.exe -i inputFile -o outputFile [-c configFile]

```

For now, poc only support one DICOM file as input. 

# Configuration File Sample

If `-c configFile` is not given, the tool will default use "configuration-sample.json" in the same directory with exe tool. For now, configuration file is just a sample used for POC testing.

The configuration file has two sections, more details of configuration could reference [DICOM de-id design spec](https://microsofthealth.visualstudio.com/Health/_git/health-paas-docs?path=%2Fspecs%2FDe%252DIdentification%2FDICOM%2FRefine-De-id-Dicom.md&version=GBfeatures%2Fdicom-de-id&_a=preview). As for dicomTagRules, only tag value and VR are supported for now.
```
{
  "dicomTagRules": [
    {
        "tag": { "value": "(0028,0030)" }, //Pixel​Spacing, decimal string type.
        "method": "perturb",
        "span": "1",
        "roundTo": 2,
        "rangeType": "Proportional"
    },
    {
        "tag": { "value": "(0040,1001)" }, //test for sequence. All tags will be redact within nested data.
        "method": "redact"
    },
    {
      "tag": { "value": "(0010,0020)" },  // patient ID
      "method": "cryptohash"
    },
    //Locate tag by tag VR.
    {
        "tag": { "VR": "PN" },   //Patient Name
        "method": "encrypt"
    },
    {
        "tag": { "VR": "DA" },   //Date
        "method": "dateshift"
    },
    {
        "tag": { "VR": "DT" },   //Date Time
        "method": "dateshift"
    }
  ],
    "parameters": {
        "dateShiftKey": "123",
        "dateShiftScope": "SeriesInstance", // Scope could be SeriesInstance, StudyInstance and SOPInstance
        "dateShiftRange": "50",
        "cryptoHashKey": "123",
        "encryptKey": "", //If empty, will use random key.
        "enablePartialAgesForRedact": true,
        "enablePartialDatesForRedact": true
    }
}

```
# Detailed Usage

For now, POC supports 5 de-id functions:

## Redact
Behaviors:
    
1. Supports partial redact for date and date time. (Maintains values of year)
2. Supports partial redact for Age. (Maintains values if less than 89 years old)
3. Redact the entire tag for other types of data.

Here is a sample configuration:

```
    "dicomTagRules": [
        {
            "tag": { "VR": "DA" },   //Date
            "method": "redact"
        },
    ]
    "parameters": {
        "enablePartialAgesForRedact": true,
        "enablePartialDatesForRedact": true
    }

```

Input:
```
"(0008,0020)" : "20161012"
```
Output:
```
"(0008,0020)" : "20160101"
```

## DateShift

Dateshift function can only be used for date (DA) and date time (DT) types. In configuration, customers can define dateShiftRange, DateShiftKey and dateShiftScope. The valid scope valid is SeriesInstance, StudyInstance or SOPInstance. The date within the same scope will shift same days.

A sample configuration:

```
    "dicomTagRules": [
        {
            "tag": { "VR": "DA" },   //Date
            "method": "dateshift"
        },
    ]
    "parameters": {
        
        "dateShiftKey": "123",
        "dateShiftScope": "SeriesInstance", 
        "dateShiftRange": "50",
    }

```

Input:
```
"(0008,0020)" : "20061012"
```
Output:
```
"(0008,0020)" : "20061201"
```

## Perturb

Perturb function could be used for any of numeric values including (ushort, short, uint, int, ulong, long, decimal, double, float).

Here is a mapping between DICOM VR and numeric types after transformation.
|VR|VR name| numeric type|
|---|---|---|
|AS|Age String|int|
|DS|Decimal String|decimal|
|FL|Float Point Single|float|
|OF|Other Float| float|
|FD|Float Point Double| float|
|OD|Other Double| double |
|IS|Integer String|int|
|SL| Signed Long| int|
|SS|Signed Short|short|
|US|Unsiged Short| ushort|
|OW|Other Word|ushort|
|UL|Unsigned Long|uint|
|OL|Other Long|uint|
|UV|Unsigned Very Long|ulong|
|OV|Other Very Long|ulong|
|OL|Signed Very Long|long|

>Note:
 UV, OV and OL are not defined in DICOM standard but defined in fo-dicom library.

Here is a sample perturb setting in configuration. Details of these settings could refer [FHIR's perturb setting.](https://github.com/microsoft/FHIR-Tools-for-Anonymization/tree/6a9b8614c319afb5f85959c02f86b2304ec4618c#Perturb-method)


```
    "dicomTagRules": [
    {
        "tag": { "value": "(0028,0030)" }, //Pixel​Spacing, decimal string type.
        "method": "perturb",
        "span": "1",
        "roundTo": 2,
        "rangeType": "Proportional",
    },

```


Input:
```
"(0028,0030)" : 0.58984375\0.58984375
```
Output:
```
"(0028,0030)" : 0.74\0.75
```

>Note:  BACKSLASH "\\" used to concatenate multi values.

## Encryption

We default using sysmetric AES encryption method in DICOM. (Asysmetric encryption is also supported in de-id lib.) Customers can set encryption key in configuration file. If encryption key is not given, the tool will randomly generate a string as key. The acceptable key sizes are 128, 192 or 256 bytes.

Customers should take care of the length of the output since all DICOM tags have maximum length limits.

A sample configuration:

```
    "dicomTagRules": [
        {
            "tag": { "VR": "PN" },   //Patient Name
            "method": "encrypt"
        },
    ]
    "parameters": {
        "encryptKey": "",
    }
```

Input:
```
"(0010,0010)" : "Annie"
```
Output:
```
"(0010,0010)" : "zp/pSrmzmxm5Eh6jj6ocBVfw39f/V8nCMwk/kgvXc14="
```


## CryptoHash

The usage and settings of cryptoHash are similar with encryption. DICOM will use Sha256 as default methods which has 64 bytes output. 

Customers should take care of the length limitations when using.


A sample configuration:

```
    "dicomTagRules": [
        {
            "tag": { "VR": "PN" },   //Patient Name
            "method": "cryptoHash"
        },
    ]
    "parameters": {
        "cryptoHashKey": "123"
    }

```

Input:
```
"(0010,0010)" : "Annie"
```
Output:
```
"(0010,0010)" : "b4a3161dca74ff66687faf324a2db061282dee979d7ad7614eaae7d4d7b9301f"
```

