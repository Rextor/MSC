# MSC API For multi site cracker
- The refrence for Request and Response from web...
- Make your crackers by it to easy
-                                                          [Ju$t MiLLie]
# Config
- you can set your setting on config for request
- Config config = new Config();
- config.LoginURL= "www.google.com";
- config.Cookies = "key=value; key2=value2";
- config.AddHeader("Name:Value");
# Requester
- Requester class just use for config to request and return a RequestManage
- Requester Rer = new Requester();
- RequestManage goole = Rer.GETData(config); //GET Data
- RequestManage google = Rer.POSTData(config); //POST Data (but you should set POSTData in config)
# RequestManage
- The all request has a result.
- for get returted StringStream use google.SourcePage
- for get cookies use google.CookiesString
- for get HTTP Code use google.StatusCode
- for get HeadersCollection use google.Headers
- and other...
# MSC
- PE-Ret
