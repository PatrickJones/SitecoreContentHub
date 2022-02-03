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

const string ENTITY_NAME_TO_CREATE = "PJ.Lion"; // Update with entity name that is getting created

var exeSource = Context.ExecutionSource;
var exeType = Context.ExecutionType;

var targetId = Context.TargetId;
var targetType = Context.TargetType;

var loadConfig = new EntityLoadConfiguration(
    CultureLoadOption.Default, 
    new PropertyLoadOption(GetPropertyLoadOptions()), 
    new RelationLoadOption(GetRelationLoadOptions()));

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

    decimal imageWidth = (decimal)fileProperties["properties"]["width"];
    decimal imageHeight = (decimal)fileProperties["properties"]["height"];    

    var pjLionEntity = await MClient.EntityFactory.CreateAsync("PJ.Lion").ConfigureAwait(false);
    
    pjLionEntity.SetPropertyValue("Name", fileName);
    pjLionEntity.SetPropertyValue("Width", imageWidth);
    pjLionEntity.SetPropertyValue("Height", imageHeight);
    pjLionEntity.SetPropertyValue("HeightString", imageHeight.ToString());

    var entityId = await MClient.Entities.SaveAsync(pjLionEntity).ConfigureAwait(false);
    if (entityId <= 0)
    {
        MClient.Logger.Error("PJ Lion entity not created.");
        return;
    }

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
            Height = imageHeight,
            NewEntityId = entityId
        });

    MClient.Logger.Debug(logData);
}
catch (Exception e)
{
    MClient.Logger.Error($"Unable to parse asset properties and create new entity. Error: {e.Message} Stack: {e.StackTrace}");
    return;
}

// Property load options for asset/entity that was just created
// Update with property names that exist on incoming entity
string[] GetPropertyLoadOptions()
{
    return new string[] { "Title", "FileName", "FileProperties", "ApprovedBy" };  
}

// Relation load options for asset/entity that was just created
// Update with relation names that exist on incoming entity
string[] GetRelationLoadOptions()
{
    return new string[] { "TagToAsset" };
}