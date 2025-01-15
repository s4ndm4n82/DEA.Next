-- Insert into CustomerDetails
INSERT INTO CustomerDetails (CustomerStatus, Token, UserName, TemplateKey, Queue, ProjectID, DocumentId, DocumentEncoding, MaxBatchSize, MainCustomer, ClientName, SendEmail, SendSubject, ClientOrgNo, ClientIdField, IdField2Value, ClientIdField2, FileDeliveryMethod)
VALUES
(false, 'eyJhbGciOiJ...', 'import_efacto', '', '0', 'd33f2483-c79f-4110-a0ab-e234fbcf37a0', '', '', 1, 'Aksesspunkt', 'Duett', 0, FALSE, '932971917', 'Internal ID', '', '', 'FTP')
RETURNING id INTO :customer_id;

-- Use the generated ID for related tables
DO $$ BEGIN
    -- Insert into FtpDetails
    INSERT INTO FtpDetails (CustomerUUID, FtpType, FtpProfile, FtpHostName, FtpUser, FtpPassword, FtpPort, FtpFolderLoop, FtpMainFolder, FtpMoveToSubFolder, FtpSubFolder, FtpRemoveFiles)
    VALUES
    (:customer_id, 'FTP', 'profileepe', 'ftp.digiacc.no', 'ftp.digiacc.no|EHFPortalConvert', 'Nt*N-9&z!V9]mPve', 21, 0, '/Navitro_importvic/932971917', FALSE, '', TRUE);

    -- Insert into EmailDetails
    INSERT INTO EmailDetails (EmailAddress, EmailInboxPath, SendEmail, SendSubject, CustomerDetailsId)
    VALUES
    ('', '', false, false, :customer_id);

    -- Insert into DocumentDetails
    INSERT INTO DocumentDetails (CustomerUUID, DocumentType)
    VALUES
    (:customer_id, 'MULTI');

    -- Insert into DocumentExtensions
    INSERT INTO DocumentExtensions (CustomerUUID, Extension)
    VALUES
    (:customer_id, '.pdf'),
    (:customer_id, '.jpg'),
    (:customer_id, '.jpeg'),
    (:customer_id, '.tif'),
    (:customer_id, '.tiff');
END $$;
