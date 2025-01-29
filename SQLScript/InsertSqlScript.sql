-- Insert into CustomerDetails
DO $$
DECLARE
    customer_id UUID;
BEGIN
    INSERT INTO public."CustomerDetails" ("Status",
    "CustomerName",
    "UserName",
    "Token",
    "Queue",
    "ProjectId",
    "TemplateKey",
    "DocumentId",
    "DocumentEncoding",
    "MaxBatchSize",
    "FieldOneValue",
    "FieldOneName",
    "FieldTwoValue",
    "FieldTwoName",
    "Domain",
    "FileDeliveryMethod",
    "CreatedDate",
    "ModifiedDate")
    VALUES
    (FALSE,
    '<CustomerName>',
    '<UserName>',
    '<Token>',
    '<Queue>',
    '<ProjectId>',
    '<TemplateKey>',
    '<DocumentId>',
    '<DocumentEncoding>',
    '<MaxBatchSize>',
    '<FieldOneValue>',
    '<FieldOneName>',
    '<FieldTwoValue>',
    '<FieldTwoName>',
    '<Domain>',
    '<FileDeliveryMethod>',
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP)
    RETURNING "Id" INTO customer_id;

    -- Use the generated ID for related tables
    IF LOWER((SELECT "FileDeliveryMethod" FROM public."CustomerDetails" WHERE "Id" = customer_id)) = 'ftp' THEN
        -- Insert into FtpDetails
        INSERT INTO public."FtpDetails" ("FtpType",
        "FtpProfile",
        "FtpHost",
        "FtpUser",
        "FtpPassword",
        "FtpPort",
        "FtpFolderLoop",
        "FtpMoveToSubFolder",
        "FtpMainFolder",
        "FtpSubFolder",
        "FtpRemoveFiles",
        "CustomerDetailsId")
        VALUES
        ('<FtpType>',
        '<FtpProfile>',
        '<FtpHost>',
        '<FtpUser>',
        '<FtpPassword>',
        '<FtpPort>',
        FALSE,
        FALSE,
        '<FtpSubolder>',
        '<FtpMainFolder>',
        TRUE,
        customer_id);
    ELSE
        -- Insert into EmailDetails
        INSERT INTO public."EmailDetails" ("Email",
        "EmailInboxPath",
        "SendEmail",
        "SendSubject",
        "CustomerDetailsId")
        VALUES
        ('<Email>',
        '<EmailInboxPath>',
        FALSE,
        FALSE,
        customer_id);
    END IF;

    -- Insert into DocumentDetails
    -- Only add the extensions that are supported.
    -- If client is only going to be sending PDFs, then only add the PDF extension.
    -- Remove the other extensions.
    INSERT INTO public."DocumentDetails" ("Extension", "CustomerDetailsId")
    VALUES
    ( '.pdf', customer_id),
    ('.jpg', customer_id),
    ('.jpeg', customer_id),
    ('.tif', customer_id),
    ('.tiff', customer_id);
END $$;