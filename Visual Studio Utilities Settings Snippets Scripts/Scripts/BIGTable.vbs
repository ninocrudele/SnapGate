url="http://localhost:8000/HubGate/ExecuteTrigger?TriggerID={9A989BD1-C8DE-4FC1-B4BA-02E7D8A4AD76}"
Set objHTTP = CreateObject("MSXML2.XMLHTTP")
Call objHTTP.Open("GET", url, FALSE)
objHTTP.Send