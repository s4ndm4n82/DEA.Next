# This script will convert the old Json config file to the new Json config file

# Read the old Json config file
$scriptDirectory = $PSScriptRoot
$oldJsonFile = Join-Path $scriptDirectory "CustomerConfig.json"
$newJsonFile = Join-Path $scriptDirectory "CustomerConfig_New.json"
$outputFile = Join-Path $scriptDirectory "CustomerConfigConverted.json"

# Read the old JSON config file
$oldJson = Get-Content -Path $oldJsonFile -Raw | ConvertFrom-Json

# Read new JSON structure as a template
$newJsonTemplate = Get-Content -Path $newJsonFile -Raw | ConvertFrom-Json

# Initialize an empty array for transformed customer details JSON
$newCustomerDetails = New-Object System.Collections.Generic.List[PSObject]

# Loop through each customer in the old JSON
foreach ($customer in $oldJson.CustomerDetails) {
    # Create a new customer detail JSON object
    $newCustomer = [PSCustomObject]@{
        Status = if ($customer.CustomerStatus -eq 1) { $true } else { $false }
        CustomerName = $customer.ClientName
        UserName = $customer.UserName
        Token = $customer.Token
        Queue = $customer.Queue
        ProjectId = $customer.ProjectID
        TemplateKey = $customer.TemplateKey
        DocumentId = $customer.DocumentId
        DocumentEncoding = $customer.DocumentEncoding
        MaxBatchSize = $customer.MaxBatchSize
        FieldOneValue = $customer.ClientOrgNo
        FieldOneName = $customer.ClientIdField
        FieldTwoValue = $customer.IdField2Value
        FieldTwoName = $customer.ClientIdField2
        Domain = "$($customer.DomainDetails.MainDomain)/$($customer.DomainDetails.TpsRequestUrl)"
        FileDeliveryMethod = $customer.FileDeliveryMethod
        DocumentDetails = New-Object System.Collections.Generic.List[PSObject]
        FtpDetails = @{
            FtpType = $customer.FtpDetails.FtpType
            FtpProfile = $customer.FtpDetails.FtpProfile
            FtpHost = $customer.FtpDetails.FtpHostName
            FtpUser = $customer.FtpDetails.FtpUser
            FtpPassword = $customer.FtpDetails.FtpPassword
            FtpPort = $customer.FtpDetails.FtpPort
            FtpFolderLoop = if ($customer.FtpDetails.FtpFolderLoop -eq 1) { $true } else { $false }
            FtpMainFolder = $customer.FtpDetails.FtpMainFolder
            FtpMoveToSubFolder = $customer.FtpDetails.FtpMoveToSubFolder
            FtpSubFolder = $customer.FtpDetails.FtpSubFolder
            FtpRemoveFiles = $customer.FtpDetails.FtpRemoveFiles
        }
        EmailDetails = @{
            Email = $customer.EmailDetails.EmailAddress
            EmailInboxPath = $customer.EmailDetails.EmailInboxPath
            SendEmail = if ($customer.SendEmail -eq 1) { $true } else { $false }
            SendSubject = $customer.SendSubject
        }
        CreatedDate = (Get-Date -Format "yyyy-MM-ddTHH:mm:ssz");
        ModifiedDate = (Get-Date -Format "yyyy-MM-ddTHH:mm:ssz");
    }

    # Map the document details
    foreach ($ext in $customer.DocumentDetails.DocumentExtensions) {
        $newCustomer.DocumentDetails.Add([PSCustomObject]@{
            Extension = $ext
        })
    }

    # Add the new customer to the customer details array
    $newCustomerDetails.Add($newCustomer)
}

# Assign the transformed customer details to the new JSON template
$newJsonTemplate.CustomerDetails = $newCustomerDetails

# Write the new JSON structure to the output file
$newJsonTemplate | ConvertTo-Json -Depth 10 | Set-Content -Path $outputFile -Encoding UTF8

Write-Host "Conversion completed. Output file: $outputFile"