DECLARE @var0 nvarchar(128)
SELECT @var0 = name
FROM sys.default_constraints
WHERE parent_object_id = object_id(N'dbo.Customer')
AND col_name(parent_object_id, parent_column_id) = 'Turn';
IF @var0 IS NOT NULL
    EXECUTE('ALTER TABLE [dbo].[Customer] DROP CONSTRAINT ' + @var0)
ALTER TABLE [dbo].[Customer] DROP COLUMN [Turn]
DECLARE @var1 nvarchar(128)
SELECT @var1 = name
FROM sys.default_constraints
WHERE parent_object_id = object_id(N'dbo.Customer')
AND col_name(parent_object_id, parent_column_id) = 'LoadingTime';
IF @var1 IS NOT NULL
    EXECUTE('ALTER TABLE [dbo].[Customer] DROP CONSTRAINT ' + @var1)
ALTER TABLE [dbo].[Customer] DROP COLUMN [LoadingTime]
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
INSERT INTO [__MigrationHistory] ([MigrationId], [Model], [ProductVersion]) VALUES ('201312190440168_AutomaticMigration', 0x1F8B0800000000000400ED5DDD72DC3A72BE4F55DE616AAE92AD5A8D7D923AB57B4ADA2D5BB24F5439B6148DEC54E5C645CF40127338E484E4B8AC67CB451E695F61F9837F347E892139B1EE6600B0D98D6E028D46E3C3DFFEF7FFCEFFFA7D972DBEA1B24A8BFC62F9FAECD57281F24DB14DF3C78BE5A17EF8E39F967FFDCB3FFEC3F9BBEDEEFBE23369F753DBAE7932AF2E964F75BDFF65B5AA364F68975467BB74531655F1509F6D8ADD2AD916AB9F5EBDFAD3EAF5AB156A482C1B5A8BC5F9DD21AFD31DEAFE347F2F8B7C83F6F521C93E145B9455B8BCA9597754171F931DAAF6C9065D2CDF2687A7E4509DF52D978B37599A345CAC51F6E0C9D2AB3FB72C2DE9CB9AD7BD6BD8AA9FEF9FF7A87BE5C5F25385CADBB2784833C4376C9AFE3B7A160A9AA2A6E11E95F5F31D7AE01EBFDE2E172BF1D995FC307D547AAEE5E462799DD7FFF2D372F1F19065C9D7AC297848B20A2D17FB9F7F59D745897E45392A931A6D6F93BA4665A39FEB2DEA24C13DF2CBFE67B74EF9F3EAD54F6DA7AC923C2FEAA46E94AD300FB0DAFE22CCAEEBB2B19DE5E27DFA1D6D7F43F963FD4419FE907C2725CDCFE5E2539E36A6D63C549707C40BD8FF37BFF87DD37A9217BFDB256976E4B79EAF98291A0DF4F250D5C50E9521D6797DE56F99ED33D42A7FFE572FABFC58E468048B9CC428D64986FEAB934FF3CDBA1069D8F8FCEEDF46E7FDBE4CF26A5F94F5E86FFE90A4F9E5FAEEE6C3E86F7E9B6C7E3FECA77977F38DBDD96C8A660E24AF7E5B14194A726FC5A164373AF7976FAFEED07EF4D7FEFA69F457BED996A8AA467FED6DD20DF093BC1795E31B54F3DE2B546DCA74DF8FEE23BFFE2AAD846F714CB99B79DB329B3A8D9F6FEEA61B3F2778F7CDD7C6E7FC968C632D1F936FE963F72E898BBBE25037CDEF50D6D5564FE9BE5F869C119FEC0B6EF2BE2C767745C6796B7DCD9775712837ED875E80D5F749F9886A778E9A65549D6CEACACC146B05F0452AF5ACD116BEDCDD94DB66F908F2D6557DA1AE2CC7985843DF49B892AA09D3419E74AFAC29DCE8F92EEE2671A57F43C9F63EDDA1FB74F37B356C7CBCCD92A30FECEE4BB5FECB793131D1C92A5132BA894D62D7F72843FB276E8DF863462CBA313BE4235827FB90EF003F16FA298C14B4B82A3657494D4DA3FDDD8E81DE23DE0FB5722013FFEDCDE8AFBEBBBA7A3F58599FD153BAC9D0243ABB4CF20DCAEED07FA34D7DB9A6A2148786825D10BD0F4A9DB8217E9EEC7D6ADC4057A6FEE390B4A3508A4CEE27DF48668CD56958E31AF8327785B2F41B2A9F0DACB1263263A446C316ADF6656ADD8C5A07536F910632437DB9861D5CE9CD4CF3F21D6A1D39801D52F985AC2F18475295B27A90EBA1E58389AFEBFC5B916E8C46C59AC81D456A345D45AB7D3BEBF6E6CAC04F572BB3D2146AB8686B20069C277BFC5D3CBF38BDE280543FFB8FB92A8DF5A781343EA2FA3F51FAF8540FA4F3B9C80ECCA37622E26C4274F07BD9E1A2534690A7183A3DD249049E21816A653481DA0C1A57F0F4F332AA88F1BFAD7EFFD1E93B6EBAF57138897A20893B94542CB41B46E3668FF2EB1AD13509B0C3E6C80BBF311A284FB356B82CF287B4DCA1AD1F43EE1F04F1915E4649D751D229667328C7DF91BA4CCAF2F93D1A7F4DD8FBC1F7C5AF85A7C16BA7962EB520AD9BEE4FCCDEBBD01070E1B97A6576011B796F9E349D9E6AD6ADF405B411C021AED373471AF87286A30466CE682380335CA7E78C3488B8A5E3B12CD3B2252DDB82C6C5EB7C7F7809B9CB7EC2AE5F4E8F9D4F71A89F8A72F4D7F6E3BE97B7ECBEA7830784975977DCAD15DB60A9D903C723306BC46D814B75EA0EB8DCC03BD057B66B203367B48DCA18AED2F245EA4326BED6DB3033C6B55259A3955AE6588B41A33999E55EC67328583B648D729B71DE7294EF769AED2A4FFFB4FF6840CB274E1169C2CC5EAC516C5EAA1E64F0F8E52FF6FEC3ECDE7BB91EDD2AF1C53880B4BC91AD63D29DDCA29A4D8E13DEB67AF18707C6EA67B15FE4ACF57657F165141A167B74CF53ED23D201FDDD3F19D2EBECC913E8FB49DC83CB1275CC45F9DC6338F49FF659916CD1F8E19646E54FE3A763351654FE3E9759F0B7E2F1653C14D9BC1DDD24263B98FC6633CD3926FF914767D06FAAAAD8A49D62A5B4BE2FD0298977F976613EDFD2F7047F0CA511FE90D5E93E4B370D0317CB3F28E269A9D2342E46153BFD22C9576767AF657939C91C05A6C765ACDCA96767EC62AB3C5A6803C2937305E61EF5905D4AD3D431A7CBD9649CF5A9DE1E9AD61DF7B177E4EBA53CBEDEE457CD92BD468BFE7B6C17CCD526D9AADF51F3116C877414971663960B4A9019DC5940460D4794E6DFC5FC36A084201D93C6EC20C627CB707597DF945474EC2E90F272CD2A529374076B5D49EC75E8CB0802836957326F720AF06061A5B4618E207194230A0A6624E8F833A727707CD20C66AFE1DF98D8C091EF37B9A38DFF52E45BC79D2E0CCEF8A24909EE1AD704CF79F3C6F1F89893BDBC2FA89D8FB59B84DC0C457244FC667ADDEEA2777706C84D761D6DCC295B90B1A496F72EADEA1E2E33DBD0B43107EC6EC6925BDD169548F76960B16457F298AC038F92D4641FD34C5FB736172AA44743048737900106959CA94882CBA956F6CF3B8EE0380DCBCA9F9C93156F0E930FE1F8B905DEAE0A3D7963F62DD4633816BE2CE26A4EEF089375BF4512595E35062F33251CF319EC94F14783386ADD01A36003EEC311DD8236CD518905C4907FEF746B8DF69935AAB9B8CF6D0FD9D778862CC08119E411FD5419455264B109D1610B510B912E360151C0410B1B0F14AE4365812CFC2D24480AA64200ABDEF2387FEC4321C15657162A78550253614B160B95DEE1076990B5808D021E8F40127488B310E91C6E8802F6C46D3AEDA73850A764F6B39060296C0A093A8FD8344272CD5475609FCB410C9C1306CAD13B2FD6AE24272481CEC423A5854433E6408F774391EDE3EC36B6C0AF131FFCB010F8AD78849E6E7700E447B9314F1D62085610D7481A68A0D0AB4BF095722C8C672B673A6490E7E890214D0EA48912FA48CF46394307C0A158C7606C783728B1579E14E57B705FC887E3D58E30C5655D22B31CDF644A30C8AF89C5BAF463B0F4FC54A3935F1774740BB886F401105FE4C8F03C0FEE06F0ACA5DA13D608AC730C9613849FA10D7D620ABB1EAB5B6418049D6DC0015997906C885D284158A7CE0C169F38383AE1A1E0AC3D3C1B22B81490E54830FF6CB0D0F0D13055787BC0D63D64CB4BC2FC44437F1823B41C35E2300EEE15392B59ED0F53F4D6257ECB71CD9C4C43176822B6FCB7401CCDE1FE8272800370178CC15CB7702E3FC9515FDDE42BE8A2B70E9D39A01B68C7EA7B010CED3A057743FB408EE51ED510B82588BE0F34A15EC7606F683FA8B15D8912E63BDE4049D78E8641128CFCBAC57EC3074739DAEBD0A543FA812E800DFD000682DD42C1E1FD20077F8F333A28476F0DDD0085859D02C3E19D20C581ADFE47B0BFC4220B3A8F098E11BB448943BC2625282CF80784D748C277C10F9DDC4AACD8122D0E91968F0F73CFF7B11A672149861A0D09D3BAF3557F370C2E385F692E9139FF90ECF769FEC85D2A834B16EBFE4699CB3FAEFD6F8ED9F534561BC178E400367D535D94C923926A7BC496F76959757EE3D7A4CDCFBBDCEE94668E0170F236200EAE6A8B04ACC843ED6F21DC8E6FD839D35E81C3F7E5FB46BCF6EBEE24459CBE8D4F2FDA0B7E922C2981D4CDF6FA9BCB16562A17CB64C33353E91340653A7DA93B2576D50C4F8995BA53C248AC3C195CA4D2385F499D2B2B71A56851FAAC64CB70B21B16D5196034BA209583C5E81FD5756A9B03CDF7A89A476D7A5A55ACAF52D9DD2F3C1556EA4E895C00C3D32165EE54B8FB5C78425CB10F47F4861691295AEC4E4BB87385A72654B8D3E3EF51E1C9F1E51EBDD65DA622745857E24E81DC8CC2D32065EE54DA8B4E780AED7FF7A7C99D253C0552E64E855E41C293A1857E743A6060994E57E84547B822442227D4B95365377FF0F458A9177FEDBA4B624BDD4233D1E0EEF200BEB4B638E04B93A90915EEF4841B37787A42C56CA630BC3F3560FE02F7DA1C262FCD73739DB9A4AB1678525295C7B7D0DFB9207C0B7DD16CECC3B091E8EEE140B73938D988F6C9E358497FC182301F742563DA19079E204EB0B4F8FF950BAD8DAA385B1770494247C2665B9AE7F49EEB5E362EF09605130D7A7381308992420F1B511C8EA9BC0DFE5A01C197E3CADDA9F53705F074FA12770A02B8044F48A8F0904F82FE176494EA66F34D1977BA9DBF2B188FBCA362FBB4F48F1E67DCC6980F3C015CE44563FD49A1B1F65A517018E1C214C08A3D2C19E3840B468CCB666369DCD6FD004BD3A52238589AFED1E3581A307C6BC6EE893442B30A06E803CE8E70D086EEC1E3E8A2C7F01646E4423DB36AA28031BC8539BD2FF2A2219B042EF298F7308AB730F3E1328F9527C5F116969DB4D4871F35240627791AA98858DEF2B4CE55CDE7EBA1FB73433E1F105FBCA362FD80B48F4E3D9C695DD10EF55B7045BB121F178BA43F8BCE159C146DA2C463720B9F00573E1B53C3694E03EC0CC06BEE48D88C4CF3DCB106690CA02C8ED33BE8EC80890E01441616E7B8CCC35A53D9A5EA4B66631534C76348B805CE5B7109B7E89E9C3A283791360C7971CEDAD064CF386843FBE471B4417266654F463D2864A282C171A590E6B1C21B532D7BB49982EE6B1E30E5D465C5A37970EA4F34663C73C2C15797FCE835FAAA58B34EAAD53F7A1CE5E2735482630E1DB434D1881E70EB105A4567A19AD386882111CEC37303A05F9D4C44FBE45CD7076387E626B28A2E576F80450069860ED6003E35B5254CB5918D8FA70E50020417EBA407DD83E6F88AAC101DE6AC89D2F0F959446315065EA1666C479281B20A2989B4D467AFAB45591537BADA129F78580F992A6AAB2F9BCD07D01DAF1E60FD0A126B47C066FAE053C719825A6054E1E9DB69525C0968A9108AD8786754CD66401533CAE10CD72FDC5D01B0DB2B35744D556D93E1357BCAE2A17AB55B9C4CACA701195A0766499EF6E60AE7FB0772E5CDCF65916FD30EC8E6BA6A716A2946AD8BACF2A181702B6067F8ED86C0DA3A66F5983A5DC62808EC774A2FA641C8A007B3B20983C483CD42866130A7E8700D9D5271802E87711666600B308A842363EE99457AAE22E9924722B068936F3A4C9F2A3842A0463942D174AAE2408CA855D78FDC28F860D3008136ACC91C460331814FC87E921E4363166662400C99D544705C1B9151472C83076B386CE8906145027B9C9289366CC8A029331C340C42473207FE6E1683319066C34C41045A0934044C249A19880032333402ADC0834D00C6A4B126CD88CD9D521780DE3721D7849A062119C1384C5038B39A340C420F3610199EC7B6834E1B3AEE9303FD0EE3EF844E1A3D9108E600A30BCDCA10B4C20E8F2D283845B6C416AE69B829E8F08C421793985C8CB88206206956F6A097379A41500424AB3DD0966EB912863E97809DE6630C1252D48F660B1C0694D51AB8B6AE0916868E5740AEE663130A6AD68F62152A8A97DDADA44D5D53AE4D6E9B04D235BD49E880C87E388BA07866768BA04D635884045716D8EF844A4C8B9020D96665117A79E3590441E1B21B0469191A87D0E0BFCD69BD2902CACD311E71C405A70C7567094BB1868E4978DA28900C8917DEEF38AA14D0FB56DEE6E84318C48D640D1DF69FC510BA36C382931CC25F6007B714A2299D032C9CE118008B6A57B802802837A18934B884FEA70088187C504045ECBAA4C538ECBAA2C24088321A61DF64B96824FF966E5B24C2F57355A3DD59DBE06CFD3FD9659676479E48830F499E3EA0AABE2F7E47F9C5B2054B5C2EDE646952F5F8935E388BF49AE9AADA6600CA626BE15C1ED42D8432E876F1368C2D68BF7C9B3CD75FFC91B63D61BD6ADBF71A77E916EBFC5B526E9E92F29F76C9F77FF6BD229A21164620864157BC2939DFA2AEB9807884ABD4BFA68F9D323D7515AD6B19742067597E24086A60046E38DCC008D438E4C008D404E4C008F478E840620BDE7DDFE30646E08680074620D5A208462043A0042390A27082916875203F716809788211483248C138FC91E850F048C5810AC6FC0823D113400523D06329A48EC3A9F31405C0FD8D313F1DC5D388367B49C881830C1523071ED1C980D2764F56873DB25F041D463306EE48EC843E2730B5F289B23A131D6057C052DACDAA3481019B61E1C7C001CEC97428625F4F62DBFCAE3B18094F3AB39CD379E4BE18134A07E037B0A384B3C431649490FB7A920F5991F81B83D3D7E14449CAD00C5ECF08897DEEDF2BF0A1B0E4B060668480EE1066485428B65F0263159EECB4860F570FB1687CB67A08090EF17008198277E84EC359ED7006FA8C232651669CB021C6B95321FCBF93FD927A30C1702F0123090E22500F22402004C329DC50F04030C2E3C8051F100B9243440B7465C5DD6CC10DFF131E0B9C561B1D4C5F14678A20F5C5886770507DE106C3679A0CF139F8FC84D8632590CA7EC24325C6F38B1136C5A07E3116D4E96EF89419E8C0BA475520CCBD198F3D81E10EE7FE0053B84FF6CBE08F40858D65183DCFD2E1F1E20D6E13103B0E10BC320C1BA4DD1D7C20F7FB64CD68A631C623AB104ED73E592572102A03FB3D7A3CAC83D89BBF41808977339E2BE3ACD96711D11152F162FBC20A8EDDC97EE3FE2B33F70D540064CEAD9F74C072F6DE624F9EC2E42602D60DF5FB077B6E0CAB2ECA164F0B5917634306E3D61DCF835720E14EF66B6E11E6227479D4C4448234178154C077A233022E3757DE1783123FD80DE88253D4DDBB8E4B3E1CB23ADD67E9A679DBC5F2D5D9D96B451C4645C4A0C28458A148EB0F0AA1C60851D9DA48925D1679559749AAA2ECDD9669BE49F74926702EB572B4ECB623293DB9E60AED51DE9AAC2C9CCBBB4C787794B0F495D9C41752AF1DB50EDF1B19AA329BFA71668A408A941D45F95E3A19AA7FEDF599EAAB2CA06623588084F51645FDAF95831637F955B392ADD1A21F0EDB9547B549D4DB9FBAD3083A1EFAC4139E015C72F226A3BB1373BE06A3BBB3905317DDA9E735C60ABD468C71556FB84A7142D55B71D04653BE66339EA94B0416C31A63853356BDE16EC309556F013C1B4DF160C2005395B806EB74458A66AC72EDF589132ADC086C3682BAE543D65A8D93AD7841E7B470CE5AD7DFFA37A5DECDA7C847FBD0D5081FD3133D838BB5D4FD9FB1A6E1BB3A2654B2FE6CF008FA85F05EC7F0E0BC5C8213F4E3BC1C8739B8720AD48E76E9C7A3F690951F29F3B201AFD922D6324E7BBB627413F09A52CC684263EA1FCEE0605A13307AB0D268D9ECF5AFBFCF715AFD9BB183C6D43F8FE03A869FD767D2F18470C9C9FB79BA1B7EE7E8E74928ADDA699B437B2593362EF252BAC720126B25AFBBB133BADA7D8618230AED18017F1993F5F8B3FEF8AA1F71CEF7D1FDE453BE0CC07A7CDD3B0F1FA7A7798FF165368AD7DC987B14D5CBE70C28A9BEF0D4D56FB8377886062041E40D0CCC581DBC3EED4F74F170D951F4EE1160D3A1A0793879BACB80D5B759F0FFA22BFE9D08A8C69D8C13C0CF14FD63983C2EFFE516438E2D58FE08D6A450DBC3A85D2CB75FDB03407D1A8AD0403112F1556C0759790F77FD18F012566B79034E4F51C8937B0F01DAE0A5DA2AEB246141E59CD4808C934A0B79FCE129C47139441A575908B3F89D429B5541E459ADE50D2CAAA7BC81BB48087803ABB5BC81EC2A29F4E9DD340075F84E5B95365DD5AAD46915489FD65ADE8097BB0A795C0ED1C65536932413B56A92144E1B32495269214F9D76853CC36606C8D34A9BD960C750351A7291086432B8CEA1677ABF03EC9ABE4AD7377DAD55A9788203D48A6B60C5E24A0BF96E934521DD439D0264BB0ADBC8880FFAAA4323AE00C7465C67A1DDE694AA84BB52886A57219114E143E12CC505D7489A36A03C46C14B112686FE5A7068DC177D5A69BA12AFCA55729944117CC46397B11A248473F6C219169F14A737E122D8E182CA378CAA529AF2D22289284CB11C4A702CF1F81B157502EA76E10446E529BBE355371D8F2A229BB175029AAE8E54B606393E75BEC0A8E2119741271C9C3A0264C4701CC26EC8486229E0F9AA64C6D40FCDA6002F9EC60B1A556FDDF4A7531A900F206737708C0133E948A28037B3AA425937F4230D255E5FE910D3A4EEA8C13635811E2890C5CF0AB0A31B6ED443C4A46EB1414CC3FD6872709DE356E3704F23A6703FA44154ED2E64A4214758747197530E1755BEEF5015D2B4D70686CBF92F0C5CE084DA40884FAADCE207B8A4C65DA528DFE53842D2ABE9F432829B27514474D6FE0001B9DBD6F4226AB609A20829AFC4C58BDE623900EC32189D1700C7C243A67638F62D0C34F01A5F2F9C723506AD3B5FF5EB675CD0FC55AEC0385FDD1DF2F66C60FFEF0A55E92323D1DEE991A38D1039A66DAEF3878244B1258E4813E94CE2075427DB66CC7E53D6E943B3366CAA9B4EADD2BC59D07F4EB2036A916DBFA2ED757E73A89BF1B61119EDBE6682C3D006C24DEF3F5F293C9FDF7410E6550C111A36D3F638E54DFEF690665BCAF77BE038A586441B61C7E7585B5DD6ED79D6C7674AE96307E0E14208771FDD18B847BB7D0B2853DDE4EBE41BD2F366EF43B1C7CEAFD2E4B14C7615A6C19E6FFE36E6B7DD7DFFCBDF0147D7ACDD451D0100, '5.0.0.net40')
