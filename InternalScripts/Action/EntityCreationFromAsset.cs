using System.Linq;

using System;
using Stylelabs.M.Sdk;
using System.Net;
using Stylelabs.M.Scripting.Types;
using Stylelabs.M.Base.Querying.Linq;
using Stylelabs.M.Base.Querying.Filters;
using Stylelabs.M.Base.Querying;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

var exeSource = Context.ExecutionSource;
var exeType = Context.ExecutionType;

var targetId = Context.TargetId;
var targetType = Context.TargetType;

var loadConfig = new EntityLoadConfiguration(
    CultureLoadOption.Default, 
    new PropertyLoadOption("Title", "FileName", "FileProperties", "ApprovedBy"), 
    new RelationLoadOption("TagToAsset"));

if (!targetId.HasValue)
{
    MClient.Logger.Error("TargetId: " + targetId);
    return;
}
var asset = await MClient.Entities.GetAsync(targetId.Value, loadConfig);

if (asset == null)
{
    MClient.Logger.Error("Asset not created");
    return;
}

try
{
    var title = await asset.GetPropertyValueAsync<string>("Title");
    var fileName = await asset.GetPropertyValueAsync<string>("FileName");
    var approvedBy = await asset.GetPropertyValueAsync<string>("ApprovedBy");
    var fileProperties = await asset.GetPropertyValueAsync<JObject>("FileProperties");

    var imageWidth = fileProperties["properties"]["width"];
    var imageHeight = fileProperties["properties"]["height"];    

    string logData = JsonConvert
    .SerializeObject(new 
        { 
            ExecutionSource = exeSource,
            ExecutionType = exeType,
            TargetId = targetId,
            TargetType = targetType,
            Title = title,
            Filename = fileName,
            ApprovedBy = approvedBy,
            //FileProperties = fileProperties.ToString()
            Width = imageWidth,
            Height = imageHeight
        });

    MClient.Logger.Debug(logData);
}
catch (Exception e)
{
    MClient.Logger.Error($"Unable to parse asset properties. Error: {e.Message}");
    return;
}



