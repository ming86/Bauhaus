CREATE TABLE [dbo].[UserProfile] (
    [UserId] [int] NOT NULL IDENTITY,
    [UserName] [nvarchar](max),
    [FullName] [nvarchar](max),
    [Email] [nvarchar](max),
    CONSTRAINT [PK_dbo.UserProfile] PRIMARY KEY ([UserId])
)
CREATE TABLE [dbo].[Customer] (
    [ID] [bigint] NOT NULL,
    [Name] [nvarchar](max),
    [SaleZone] [int],
    [Route] [nvarchar](max),
    [LeadTime] [nvarchar](max),
    [MaxVEH] [nvarchar](max),
    [Transport] [nvarchar](max),
    [Turn] [nvarchar](max),
    [LoadingTime] [nvarchar](max),
    [MainCSROM] [nvarchar](max),
    [BackupCSROM] [nvarchar](max),
    [KeyAccount] [bit],
    [Team] [nvarchar](max),
    [CBDRep] [nvarchar](max),
    [GU] [nvarchar](max),
    [Adress] [nvarchar](max),
    [PayType] [nvarchar](max),
    [PayTerm] [nvarchar](max),
    [PayDescription] [nvarchar](max),
    [Discount] [nvarchar](max),
    [Payer] [bigint] NOT NULL,
    [MainCSRAR] [nvarchar](max),
    [BackupCSRAR] [nvarchar](max),
    [Observation] [nvarchar](max),
    CONSTRAINT [PK_dbo.Customer] PRIMARY KEY ([ID])
)
CREATE TABLE [dbo].[Contact] (
    [ID] [int] NOT NULL IDENTITY,
    [Area] [nvarchar](max),
    [Name] [nvarchar](max),
    [Telephone] [nvarchar](max),
    [Email] [nvarchar](max),
    [Customer_ID] [bigint],
    CONSTRAINT [PK_dbo.Contact] PRIMARY KEY ([ID])
)
CREATE TABLE [dbo].[Order] (
    [SapID] [int] NOT NULL,
    [DocDate] [datetime] NOT NULL,
    [Type] [nvarchar](max),
    [PayTerm] [nvarchar](max),
    [CustomerPO] [nvarchar](max),
    [Plant] [nvarchar](max),
    [RDDF] [datetime] NOT NULL,
    [VehicleType] [nvarchar](max),
    [CancelRejectCS] [float] NOT NULL,
    [Customer_ID] [bigint] NOT NULL,
    [Quantities_ID] [int],
    [Delivery_ID] [bigint],
    [Status_ID] [int],
    [Shipment_ID] [bigint],
    [POD_ID] [int],
    CONSTRAINT [PK_dbo.Order] PRIMARY KEY ([SapID])
)
CREATE TABLE [dbo].[Quantity] (
    [ID] [int] NOT NULL IDENTITY,
    [QtyCS] [float] NOT NULL,
    [QtySU] [float] NOT NULL,
    [NetWeight] [float] NOT NULL,
    [Volume] [float] NOT NULL,
    CONSTRAINT [PK_dbo.Quantity] PRIMARY KEY ([ID])
)
CREATE TABLE [dbo].[Delivery] (
    [ID] [bigint] NOT NULL,
    [Date] [datetime] NOT NULL,
    [Quantities_ID] [int],
    CONSTRAINT [PK_dbo.Delivery] PRIMARY KEY ([ID])
)
CREATE TABLE [dbo].[Status] (
    [ID] [int] NOT NULL IDENTITY,
    [Code] [int] NOT NULL,
    [Stage] [int] NOT NULL,
    [State] [int] NOT NULL,
    [Reason] [int] NOT NULL,
    [OpenItem] [bit] NOT NULL,
    [Report] [int] NOT NULL,
    [RDDFConfirmed] [bit] NOT NULL,
    CONSTRAINT [PK_dbo.Status] PRIMARY KEY ([ID])
)
CREATE TABLE [dbo].[Shipment] (
    [ID] [bigint] NOT NULL,
    [Date] [datetime],
    [Turn] [nvarchar](max),
    [CarryFee] [nvarchar](max),
    [OrdersToGo] [int] NOT NULL,
    [Carrier_ID] [bigint],
    [Vehicle_ID] [int],
    CONSTRAINT [PK_dbo.Shipment] PRIMARY KEY ([ID])
)
CREATE TABLE [dbo].[Input] (
    [ID] [int] NOT NULL IDENTITY,
    [Comment] [nvarchar](max),
    [Author] [nvarchar](max),
    [Time] [datetime] NOT NULL,
    [Shipment_ID] [bigint],
    CONSTRAINT [PK_dbo.Input] PRIMARY KEY ([ID])
)
CREATE TABLE [dbo].[Carrier] (
    [ID] [bigint] NOT NULL,
    [Name] [nvarchar](max),
    CONSTRAINT [PK_dbo.Carrier] PRIMARY KEY ([ID])
)
CREATE TABLE [dbo].[Vehicle] (
    [ID] [int] NOT NULL IDENTITY,
    [Status] [int] NOT NULL,
    [Plate] [nvarchar](max) NOT NULL,
    [Type] [nvarchar](max) NOT NULL,
    [Driver_ID] [int],
    [Carrier_ID] [bigint],
    CONSTRAINT [PK_dbo.Vehicle] PRIMARY KEY ([ID])
)
CREATE TABLE [dbo].[Driver] (
    [ID] [int] NOT NULL IDENTITY,
    [Name] [nvarchar](max),
    [Telephone] [nvarchar](max),
    [Carrier_ID] [bigint],
    CONSTRAINT [PK_dbo.Driver] PRIMARY KEY ([ID])
)
CREATE TABLE [dbo].[CarryFee] (
    [ID] [int] NOT NULL IDENTITY,
    [Route] [nvarchar](max),
    [VehicleType] [nvarchar](max),
    [Cost] [nvarchar](max),
    [Carrier_ID] [bigint],
    CONSTRAINT [PK_dbo.CarryFee] PRIMARY KEY ([ID])
)
CREATE TABLE [dbo].[Invoice] (
    [ID] [bigint] NOT NULL,
    [Date] [datetime] NOT NULL,
    [QtyCS] [float] NOT NULL,
    [QtySU] [float] NOT NULL,
    [Order_SapID] [int],
    CONSTRAINT [PK_dbo.Invoice] PRIMARY KEY ([ID])
)
CREATE TABLE [dbo].[POD] (
    [ID] [int] NOT NULL IDENTITY,
    [Date] [datetime],
    CONSTRAINT [PK_dbo.POD] PRIMARY KEY ([ID])
)
CREATE TABLE [dbo].[Report] (
    [ReportID] [int] NOT NULL IDENTITY,
    [Name] [nvarchar](max),
    [CreationDate] [datetime] NOT NULL,
    [Status] [int] NOT NULL,
    [Uploader] [nvarchar](max),
    [Path] [nvarchar](max),
    [Remark] [nvarchar](max),
    CONSTRAINT [PK_dbo.Report] PRIMARY KEY ([ReportID])
)
CREATE TABLE [dbo].[Log] (
    [ID] [int] NOT NULL IDENTITY,
    [IP] [nvarchar](max),
    [UserName] [nvarchar](max),
    [Action] [nvarchar](max),
    [Date] [datetime] NOT NULL,
    CONSTRAINT [PK_dbo.Log] PRIMARY KEY ([ID])
)
CREATE INDEX [IX_Customer_ID] ON [dbo].[Contact]([Customer_ID])
CREATE INDEX [IX_Customer_ID] ON [dbo].[Order]([Customer_ID])
CREATE INDEX [IX_Quantities_ID] ON [dbo].[Order]([Quantities_ID])
CREATE INDEX [IX_Delivery_ID] ON [dbo].[Order]([Delivery_ID])
CREATE INDEX [IX_Status_ID] ON [dbo].[Order]([Status_ID])
CREATE INDEX [IX_Shipment_ID] ON [dbo].[Order]([Shipment_ID])
CREATE INDEX [IX_POD_ID] ON [dbo].[Order]([POD_ID])
CREATE INDEX [IX_Quantities_ID] ON [dbo].[Delivery]([Quantities_ID])
CREATE INDEX [IX_Carrier_ID] ON [dbo].[Shipment]([Carrier_ID])
CREATE INDEX [IX_Vehicle_ID] ON [dbo].[Shipment]([Vehicle_ID])
CREATE INDEX [IX_Shipment_ID] ON [dbo].[Input]([Shipment_ID])
CREATE INDEX [IX_Driver_ID] ON [dbo].[Vehicle]([Driver_ID])
CREATE INDEX [IX_Carrier_ID] ON [dbo].[Vehicle]([Carrier_ID])
CREATE INDEX [IX_Carrier_ID] ON [dbo].[Driver]([Carrier_ID])
CREATE INDEX [IX_Carrier_ID] ON [dbo].[CarryFee]([Carrier_ID])
CREATE INDEX [IX_Order_SapID] ON [dbo].[Invoice]([Order_SapID])
ALTER TABLE [dbo].[Contact] ADD CONSTRAINT [FK_dbo.Contact_dbo.Customer_Customer_ID] FOREIGN KEY ([Customer_ID]) REFERENCES [dbo].[Customer] ([ID])
ALTER TABLE [dbo].[Order] ADD CONSTRAINT [FK_dbo.Order_dbo.Customer_Customer_ID] FOREIGN KEY ([Customer_ID]) REFERENCES [dbo].[Customer] ([ID]) ON DELETE CASCADE
ALTER TABLE [dbo].[Order] ADD CONSTRAINT [FK_dbo.Order_dbo.Quantity_Quantities_ID] FOREIGN KEY ([Quantities_ID]) REFERENCES [dbo].[Quantity] ([ID])
ALTER TABLE [dbo].[Order] ADD CONSTRAINT [FK_dbo.Order_dbo.Delivery_Delivery_ID] FOREIGN KEY ([Delivery_ID]) REFERENCES [dbo].[Delivery] ([ID])
ALTER TABLE [dbo].[Order] ADD CONSTRAINT [FK_dbo.Order_dbo.Status_Status_ID] FOREIGN KEY ([Status_ID]) REFERENCES [dbo].[Status] ([ID])
ALTER TABLE [dbo].[Order] ADD CONSTRAINT [FK_dbo.Order_dbo.Shipment_Shipment_ID] FOREIGN KEY ([Shipment_ID]) REFERENCES [dbo].[Shipment] ([ID])
ALTER TABLE [dbo].[Order] ADD CONSTRAINT [FK_dbo.Order_dbo.POD_POD_ID] FOREIGN KEY ([POD_ID]) REFERENCES [dbo].[POD] ([ID])
ALTER TABLE [dbo].[Delivery] ADD CONSTRAINT [FK_dbo.Delivery_dbo.Quantity_Quantities_ID] FOREIGN KEY ([Quantities_ID]) REFERENCES [dbo].[Quantity] ([ID])
ALTER TABLE [dbo].[Shipment] ADD CONSTRAINT [FK_dbo.Shipment_dbo.Carrier_Carrier_ID] FOREIGN KEY ([Carrier_ID]) REFERENCES [dbo].[Carrier] ([ID])
ALTER TABLE [dbo].[Shipment] ADD CONSTRAINT [FK_dbo.Shipment_dbo.Vehicle_Vehicle_ID] FOREIGN KEY ([Vehicle_ID]) REFERENCES [dbo].[Vehicle] ([ID])
ALTER TABLE [dbo].[Input] ADD CONSTRAINT [FK_dbo.Input_dbo.Shipment_Shipment_ID] FOREIGN KEY ([Shipment_ID]) REFERENCES [dbo].[Shipment] ([ID])
ALTER TABLE [dbo].[Vehicle] ADD CONSTRAINT [FK_dbo.Vehicle_dbo.Driver_Driver_ID] FOREIGN KEY ([Driver_ID]) REFERENCES [dbo].[Driver] ([ID])
ALTER TABLE [dbo].[Vehicle] ADD CONSTRAINT [FK_dbo.Vehicle_dbo.Carrier_Carrier_ID] FOREIGN KEY ([Carrier_ID]) REFERENCES [dbo].[Carrier] ([ID])
ALTER TABLE [dbo].[Driver] ADD CONSTRAINT [FK_dbo.Driver_dbo.Carrier_Carrier_ID] FOREIGN KEY ([Carrier_ID]) REFERENCES [dbo].[Carrier] ([ID])
ALTER TABLE [dbo].[CarryFee] ADD CONSTRAINT [FK_dbo.CarryFee_dbo.Carrier_Carrier_ID] FOREIGN KEY ([Carrier_ID]) REFERENCES [dbo].[Carrier] ([ID])
ALTER TABLE [dbo].[Invoice] ADD CONSTRAINT [FK_dbo.Invoice_dbo.Order_Order_SapID] FOREIGN KEY ([Order_SapID]) REFERENCES [dbo].[Order] ([SapID])
CREATE TABLE [dbo].[__MigrationHistory] (
    [MigrationId] [nvarchar](255) NOT NULL,
    [Model] [varbinary](max) NOT NULL,
    [ProductVersion] [nvarchar](32) NOT NULL,
    CONSTRAINT [PK_dbo.__MigrationHistory] PRIMARY KEY ([MigrationId])
)
BEGIN TRY
    EXEC sp_MS_marksystemobject 'dbo.__MigrationHistory'
END TRY
BEGIN CATCH
END CATCH
INSERT INTO [__MigrationHistory] ([MigrationId], [Model], [ProductVersion]) VALUES ('201312101845495_AutomaticMigration', 0x1F8B0800000000000400ED5D5F6FDCB8117F2FD0EFB0D8A7F6807A936B71680F760F8E9D5C8D5E62D79BA4405F0ECA2E6DABA795B69236883F5B1FFA91FA15AA3F24352487FF24AE24377EDB25A9D17066440E87C31FFFFBEFFF9CFEF065972C3E93BC88B3F46CF9F2E4C57241D24DB68DD3FBB3E5A1BCFBDD1F973FFCF9D7BF3A7DBDDD7D597C64EDBEADDB554FA6C5D9F2A12CF7DFAF56C5E681ECA2E264176FF2ACC8EECA934DB65B45DB6CF5ED8B177F5CBD7CB122158965456BB138BD3DA465BC23CD9FEAEF45966EC8BE3C44C9DB6C4B9282965735EB86EAE25DB423C53EDA90B3E5ABE8F0101D8A93B6E572719EC451C5C59A24779E2CBDF853CDD292BFAC7ADDEB8AADF2F1FDE39E34AF3C5B7E28487E936777714260C3AAE95FC9A3505015550DF7242F1F6FC91D78FC6ABB5CACC46757F2C3FC51E9B99A93B3E5555AFEFEDBE5E2DD2149A24F495570172505592EF6DF7DBF2EB39CFC4852924725D9DE446549F24A3F575BD2F4844AE4FBFD776E42F9D3EAC5B7B55056519A66655456CA56984758AD7F3166D7655ED9CE72F126FE42B63F91F4BE7CE00CBF8DBEB092EAE772F1218D2B53AB1E2AF303811D6CFF9B5FFCA66A3DC98B5FEFA23839F25B4F579D291A0DF4E25094D98EE47DACF3EAD2DF32EB67B8557EF7072FAB7C97A564048B9CC428D65142FED1F44FF3CDBA10B9CD0EE5F8ACFF44A2EDFB78029955143EBEFECBE8AF7D9F4769B1CFF272FC371FF2747CF566513D9B4FA4E138BD58DF5EBF1DFDCDAFA2CD2F87FD34EFAEC6D0F3CD26AB7C1CF6EA5759969028F5B61712ED46E7FEE2D5E52DD98FFEDA1F3F8CFECAF36D4E8A62F4D7DE44CD043EC97B493EBE4155EFBD24C5268FF7EDEC3DF2EB2FE342F816C7EC77E59759BC25A7F1F3FC76BAF17382775F7FAAD6149FA371ACE55DF439BE6FDE258F83595A469BB21A1F6E49D234281EE27DBBD23C616EF7CF5DAB3779B6BBCD12E093F3CA9FD7D921DFD45F7CA66BF13ECAEF49E9CEDD75BEAD16E3286F4DD5CF7C610018136BF83B1957523563BADFBAA4EDD824CB92F92E96CF73128DFE394DB21E7A4F12B27F000BA2AF7379DE7C527D3E8275B4EFF31DD0C7FA7E0A23ADD02FB3CD65D4AD74EBDFEDF2C4737AFCAADC28362EDF5C8FDFE5249AC07FBABDBC7C33D8443E9287789390492CE5224A3724B925FF249BF262CDBB921D2A0AF68EE81D133EB30F99FC659744E31BB832F5B743548F7D3131F924B091CC5857A7610D34F065EE9224F167923F1A58EB9AC88CB11A0D5BBCDA97A97535561E4CD2620D6486DA720D3BB4D29B99EAE53B527FE4083BACF267E674761C49558A4B29D7633EA589AFABF473166F8C46D5359105C56A34A2E2D5BEC2BAB9BE34F0D3D4CAAC54851A2EEA1A8C016717837E178FCFAEB63820958FFE63AE4A63FD61208D77A4FC3B89EF1FCA81743E66C9A1F3E39D88389B101FFC9E3791F894D1CB3FED3B3DF249049F21916A6534C1DA0C1A57E8F4F33CAA8841A12DB1316AD9282CA3FBE124CA81246E495474D1B57E34AEF724BD2A095F09219B1C8EBCC0CDB89EFDA9D60A17597A17E73BB2F563C8FD83603ED2F328E93A4ACE7653F422CAF3C73764FC3561EB07BFCF7ECC3C0D5E3BB534DBD97159893F327BEF4243C48507F5CAEC8236F2F59B6BA1C79A752B7F016F847048EBF4DCB106BE9CD128819933DE08E18CD6E939630D02C6F93D96655AB6A4655BAF71F12ADD1F9E03FDB29FB06B97D3636F691FCA872C1FFDB530B9C4D15B76DF49A203C2F3AC3BEE868E6DB0D46C8CD211B86B04F645A53A755B546EE01DE8CBEB35909933DE46658C5669F962F57D26BEDADB3033065AA9ACF14A2D735D8B41A3399BE59EC7732C583B648D72934481B33BA7D924F3F44FDB8F06B57CE614B1269DD98B358ACD4BD5830C9EBEFCD9DEBF9A9C012FD7A359253E1BC7F469EA93EEE466C5B17D798FE556B36DF5EC0F0F8CD5CF62BFC859EBF5AEE2F328342CF6E82C6C1A91EE21EFF6C93E52EF9E7C02B29FC43DB8C849C35C90CF3D8443FF619F64D1968C1F6EA954FE307E3A566541F92F7399057FCAEE9FC74391CD9BD14D62B2B3BFE79B698E92F88F3C3A833E2F8A6C13378A95D2FABA2305222FAFD3EDC27ABEA095073CA15089E09094F13E8937151B67CB1727272F957E9A68F3942E409B26F74BA4BF91BB0E3A69EEBB94B5A8634E97C2D871D6E65B9BF93291C43AAB11E4CBA53CDC5CA797D50AB6248BD63CEBF563B189B6AA595536B11D2228902562EE17962F32585848820920CAD3D1AC76E7D16D2C3F46C7A43159A6E3B34BF874EFBF29C7E6D82290D254CD2A527356076B5DC973759065800EA35948326F7246ECE0CE4A59B48020F31B037614DDA0D7F167DEAD077CF2845EAFE1DFB8CF0FC8B77BBEC1C67F2910ACE34E1715EEF8E27BF4EE1AD7C492A179D3F074409D2BDB64DAF958BB670666289632E137D3EB36DBBCC5D9A3DF6C13CEC69CB22317AAD7F2569E55DDC3FBDCEDEFD9984336FB42F55BDD259448B75951A1FAAEA4F558071E25C7C73EA699BE6E6D6A501F89F6E938BE9F8A30A8A41005EAB89C7964FFBCC3749C662559F9935394C2CD61F299143FB7C0DB55E10751CCBE857A2AC5C297A5BB9AC32CC264DDEE1804EEAF1A929699124EBD0C76CAE0491940AD396FD3DB80DBD579B3A08D5392D30E5290B9D7BAB546FDCC9A94200C72D382C4559E61B7DEA70C420C39B58F2229B6D8C4E8740B511B118E49A0D2602B770B099652A810A0BAB33C0E8F312824BAE591850A5D56E054BA3587854AEBB1A33498336FA340071494041FA32C441A8F19A3405D699B4EDB390AD5299BBE2C24BA942C85049F086C1A61B953AA3AA8D3E4D00D9AE384F6A3F53EACA26427FE1061D2A1CE42A21A34B0C79BB1C4F268BB51833DCD0E325808FC94DD634FD7116DF9513068A96304C01E01EDA4C142134D748C2772D6859169E5438A8DD89014E75B8E8B89FD7590857CDC59158429B4E8125C047CB341D1D07F4D38D1458EBD7B0F075B5DFF757133B798611F192021324006F23C580CE8E9395512D620A27318117404CE510699982287C7128B7CB05D671B784CD125AAD8C72E9438A293307B779F4DF1BACE63F1457B84B14FC7A5982220D17928833B8D1FF6513B6F8F39BA471D614F3A4FC9200F63901150632ED360A9C879A6AA3C4C0148971024E0BA73B30C22D0041DE1B7C05CADC1BD5753F21177C1188F748B48C2498E7BAB265F4117807410E6003170C1EAA58046279DE2937D652087238F6A08C009D7CB4013AD748C57F695831A9E942851BEC30D947CF5641824D1E0A55BF8B2FFE028072C1D443A440E7C096890031ACB748B66F697831CBF3CCEE8A01CA63488018B6C3AC536FB0B410A655AFD8FDEFE52B7B6D6794C7898D325D0D9C76B52E29A827FC0780DD4F966F9AFEBB712EEB4043CFBF4168638C1F36DB4C2B9932CE788473579DDE9AABD5083169CAE34376F9CBE8DF6FB38BD073771D092C5BABD86E3E2776BFFEB36762D8DD546301E3906CBDF546679744FA4DA1683E34D9C178DDFF829AA33AE2EB63BA599630C97BD0D09E5AADA62211BF650FD5B8818D36B494EB4F7864059BEA9BA577FDD4D4F09D0B7F1E9457D2B4A944439928C57DF1972510305A562996C78662A6D4A9F4CA72D75A7D4DDCF012975A5EE9428A22724438B541AA72B49B8B212578A16A5CF4AB60C27BBE9A23A038C4617A472B018FDA33AA1D659AD50A26A66ACE96955B1BE4AED2ECC8054BA52774AF4381224438BDC6974776040325DA93B2576A905A4C3CADCA9803B2A202150EC41AB015811C834251ED2815748080282153E32E2D7428862E2C5EEB4848B1E2035A1C29D1EBCBC019283E51EB26F6E701064DF94B85360D731401AACCC9D4A7DBB02A450FF777F9A5D940029B032772AFCDE03488617FAD1690078653A4DA1171DE15E02899C50E74EB5BB6E00D2EB4ABDF8ABD7A5125BEA269B8906B84000F9D2EAE21E5F9A4C4DA870A727C0FC437A42C57CA678FD469AFB0C8FA1E23744AC13BCEEC9E3CCEF2D50BDF0BD3725637A08E038B83880F2E2FF2B17521B5570B62E046CBE2161B32DCD737ACF6D2F1B178A566FA2C111E0854192157AD88832A14C359B40787661AE06E51E5CB588EB024F6D91875FDCE0A70B6E7153E24E4138720F0909151E329200D105394975B3F92E8DBBC5CEDF268ED2DC50B17D9EFA478F33F6D393F090002DF2A2B1FEA0D0587B799D0039599846BA620F4BA6E8C98211D3B2D9581AD8FE1E6069BAED7C074BD33F7A1C4B43A600CDF83F9146F8CEFC007DE019060EDAD03D781C5DB4C8C6C2889CA947174D1428B2B1E017B4455E346493A0451EF31EC53616663E5AE6B13AE1E8C6C2D28497FAF0A30672F05441231511E1589ED641D57CBE1EBEC735E4F34151971B2AD60F48FBE8D4C3D9F142755D12ADE85CE1A9B5264A10A958F80440F96C4C8DA60A0DB03304C5B621613332CD73C71AA429ACAC384EEFB00C74131D06132B2CF0699987B52AF1605D2078AA900DCB931812B2C1733F5C4236BA27A7DE9299481B86DC32676D6832501CB4A17DF238DA6079A7B227A31E373151A190A15218E0582192A9963DDA6C3BF7350F9AB6E9B2E2D13C38F5271A32263AE1E0AB4B20F41A7D55044E27D5EA1F3D8E72436C43070FB835B895A2B350A09EC2647E9B3699CCC3734300319D4C44FBE45CD7076387E626B28A26DF6D804520A97A0ED6803E35B5254CA40176C871801230104D273DE81E34C7576485E890384D9486CFCF2246A530F00A35633B921D54A590D6C74B7DF6CB6AEC4971B3AC2EF18987B54092A2B6DAB2D97C00CD21DD01D6AFE05336046CA68F3E759C21A8868B149EBE99264D9441390AA1888D77D6CD6C0654312B1BCF120527B7ADC9A0A0AD634E489D55AED99C564E78ABE27132354E0FB3B906DE9011E8C31BCD80EFC99B374BD57BB771836E7255D4589E1CC7D3B1C77226BEB759C887D8CD091EA0A15322072272FC94FA0C6C013F83EFC8987B5E8A9EAB40BA84E7B82DDA844D87E9533D5ADE53A38050309DAAA7E847D4AAEB476EECF860D340610AACDBF84603311DDD9767483D02C12CCCC480B730AB89E0B83622633658068FAEE1B0A1430665E829714E26D8B021434ECC70D030743A9039C0BB0A0CC6C09A0D330511A6A2A7215022C1CC4084DF98A111683B3CD80470440F6BBA84D8DC69D31A91BE09F7A3AF693092018CC3042432AB49C3D0E9C10622839BD8F64E7943C71D5244EE387A49DF49A32512C01C706C96591982B6B383CD404579B1A53480A6FD4D418706D3773149C985882B68E06566650FFAFE0633088E1F63B507DED26D97DC20730916673EC620E1EC7C6DB6001074ACD600DABA6EAD1B04AF4004CDC72614CCA1AFC52A540C24BB5BC99BBA26DB9ADC3609E2687A93D0C1387D7516C1D1A0EC16C19B86B00809ECA9A7DC1995901621015ACDCA22F4FD0D67110CC3C86E10AC65DF3884063D6B4EEB4D118E6B8EF188232E3865A0304B58AA6BE8987EA58D02C98062FDE54EA34A3DA46FE56D8E3E84A1BB81ACA1414EB31842D366587012E0A3F514704D2198D201DCDB0CC700BCAB76852BF07172139E42414BF87F0E1F47A1DB044CB9462435425C238A82C2C8C9586E6D93E5A2EAF9E7785BE3B8AD1F8B92EC4EEA0627EB7F251749DC1C76610DDE46697C478AF27DF60B49CF9635D4DC72719EC451D1A2F779A1D4F16B578B629B201875B585830C981B0CA3CDED225A1C99CD7E192D7BAEBD3820AE2561BD7AD6F75A63E956D7F473946F1EA2FC37BBE8CB6F7DAF4CEDF0DE0210A3901DDE949C6F15D6DC403AC2D5C29FE2FB46999EBA0A26DA0E780D5896E705D16DB67B00663ADCB500C418F85A0052007E2D04B5E6686708714114B62012E3386C01A809386C01E8412036F6ED78DB6A8BC216801B06C51680548DC916800C03660B408A83B305A2D540EA84A125A0B30520D901B485E18F45D37A8FEC00A22DE44718889E00D176C4F918CB701D633A3E8A63D542A805907EB0591F9C1B9CD03D4346559853AAFB8C06D815B2EA74B32ACD1ADA6658F431D4B572321D0E8DD692D856BFCB66BAF7A433CBE11C42A48560AD054A0B40A9C54B1B2872E1E86608694940692DC9BB248BFCCDCAE93B73A224A545F65E4408D974EE5F3EF2C9751959BD9911A2A8439861A118474E9CC7311C1AEEC94E90F42CEB108BA64759879000007343C8307839771ACE6AC7D3BE671CA6083277F51B629C858AC1AD3DD92FA9C56EEBEF6F50E0B64104CA410418625B7F0AD71CAB0D0D13387201C33DBDFA2182B3B9B2E26EB6E82EFB131E0B460D9D75C0682116C50019ADBFC1C0F48E213E074C0A083D5622F9E34F78A8A4F06921626F14432DC4D21C44737B4F993D1D58F7F80C067136E3B1A767E0C4591E68DEF493FD32E0B9A37E6319052BB3083C5CE4C26D02EA72F07BAF0CFB0DD2EE0E3E9270FD64CD68A6D1CA23AB10CF917EB24A0CB7B91B3C1ED6209ACDDF20D06CB719CF9561D6ECB388E808F96FA17D610536ECC97EE3FE2B33672161985E6E72D2E178D9A5D53DF9142637111F6CA8DF3FD873EBA0C1826C16D50861213664284CD8F13C780581EBC97ECD35A057009107CD0664C05E211236FCBF139D11808458795F0CBFF40D5E5C2FA036D10BA3BBC2B787A48CF749BCA95E7BB67C7172F252E917A045B31D0452AC4CA4F48D42A6B24592D7A61225D533459947B18A6D7693C7E926DE4789CCBDD4D0D1C66B91729272CD25D993B4365EA9772EAFB2604A71D2D21767938190FB6CB600096A2B88FA5F2A79EED7E965B5A629C9A2FD306A1FB4D844EAB52B4D32B88E8736990132404B9EBCC9E82EB49BAFC1E82E0B03EAE27BB650635DA1D78831AEEA0D77984DA87A2B0CD568CAD76CCB76EA12719DA8C6BAC219ABDE70A9D884AAB7E04D8DA67874EBB85395E88D37BA62453356B9F6DEB209156EC4951A41DDF21957ADC6D9A6ACA0735E3867ADEBAFDB9A52EFE643BCA37DE86AACA7D3133F0249B5D4FC9FB1A67190FC0995AC3F9A39827E31B8CD313C382F97E009FA715E8EC31C5C3905E944BBF483A0296CE5C7CABC6CC06BB608B58CD35E6B16DC04BCA6143398CB98FAC7F7F23BAD0910295469BC6CF6FAD75FA436ADFECDD02D63EA1F02688EE1E7B5395590102D79F27E9EEE6ACD39FA791248A676DA06609B6CD2A6455E4AF7184442ADE47557E50557BBCF106304011D41ED0A24E6F167FDF1553FE29CEFA3FBC9A77C19FFF2F8BA771E3E9E9EE63DC697D9285E7355E551542F679C73526DE15357BFE1C2CE191A8084503630306375F0DA0430D1C5A36547D1BB47804D0742E5E1E4E96EE154DF66815F0BAEF8D7229E15382325604F29FAA728652013E286223E2DBA4C02AA49A1B645B13A5B6E3FD54741DA8404A1816224E2ABBA1D64E53DE0F627E4255DADED0D2CAF407D01AB41E9B34A0B79FA7D28C46939469A5659087761368576578591EF6A2D6FE8826FCA1BC0752BC81BBA5ACB1BD8E68F429FDFE08150C7EF7C5469F3C5A74A9D57A1F479ADE50D7455AA90A7E5186D5A653349369FAA26C941873193649516F2DCB756C87708B608795E69331BEABFA946C3AE5BC04C86D63948A6750F50D1B4553AD9B4B556A5D27908512BADC1154B2B2DE49BBD1085740B0889906D2A2C24D9C94C852AABC008B33A0BED3A095025DC9462549B0A89A408B2A84D2B5B8076D200AFC93D139D49699E68D1D9357380F8A438FE0BF7492AC94762671C3A2A5F54A8F6D2945F15A88BC21C04C04643750F5ECCA6EBA06E374960549ED31A5E75F3D5A85DECA6345D074D37D0295B5C804FDD64396AF7D89CAAEB1C9E02816476000E7577338FD22D05835BED9931854113DC86DDD3B809A3EAAD991F744A43F6B5E55D7AC01832D58CD415F48247B553D68DE9404389D7573AC434B9BF66B04D4DC0020BC8C05901F704FB1BF5906E72BFD1D04DC3354B72901870ABF148A7E9A670CD9CA1ABDADDB440438EB02A0177DC0DEFAA7C6D9ADA49D39E111AF6855F18BA02E86B033DBAA75E0686B8A4C6DD9120DFE5389DE4375CE9FB886E0204E9A2B3F60774105CDAA4EFA226DC1DA493F25255BC2F2A9403D0DD29A1F302F0986E9FA91D8FE10A030DBE08D6774E41D8E775A7AB7681490BAABF0A92FEE9EAF690D6A79DDA7F97A488EF3B12F5D50029D9081150DEE62ABDCB583456E28835914E59BD2565B4ADC6ECF3BC8CEFAAB561555D09B588D36AC5FB314A0EA446FDFC44B657E9F5A1ACC6DBAACB64F729111C863AA06B7AFFE94AE1F9F4BA41F62D4274A16233AE0F885DA7AF0E71B2E57CBF410E886948D491627A32AFD665599FD0BB7FE494DE3590042E84A8F87880FB3DD9ED6B888CE23A5D479F899E37BB0C45899D5EC6D17D1EED0A4AA37BBEFA5B99DF76F7E5CFFF03B2C2BCA8FF130100, '5.0.0.net40')
