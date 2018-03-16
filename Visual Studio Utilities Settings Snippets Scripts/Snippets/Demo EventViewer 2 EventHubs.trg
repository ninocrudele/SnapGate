{
  "Trigger": {
  	"IdConfiguration": "{E586A527-A233-4D61-BF7F-7592DFCAC9A8}",
    "IdComponent": "{3C62B951-C353-4899-8670-C6687B6EAEFC}",
    "Name": "FileTrigger To Out",
    "Description": "Get file from disc",
    "TriggerProperties": [		
		{
			"Name": "Syncronous",
			"Value": "false"
		},
		{
			"Name": "BatchFilesSize",
			"Value": "30"
		},
		{
			"Name": "RegexFilePattern",
			"Value": ".txt"
		},
		{
			"Name": "DoneExtensionName",
			"Value": "done"
		},
		{
			"Name": "PollingTime",
			"Value": "3000"
		},
		{
			"Name": "InputDirectory",
			"Value": "C:\\Program Files (x86)\\GrabCaster\\Demo\\File2File\\In_Local"
		}]
	},
  "Events": [
    {
	"IdConfiguration": "{F4CF1B12-4EC4-43FA-8842-575A5E4CDE5A}",
	"IdComponent": "{D438C746-5E75-4D59-B595-8300138FB1EA}",
	"Name": "File2File Demo Event ",
	"Description": "Write file to the GrabCaster Demo OUT folder"
    }
  ]
}