//Script Type: Action

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

const string ENTITY_NAME_TO_UPDATE = "PJ.Lion"; // Update with entity name that is getting created

var targetId = Context.TargetId; // Get enity id from incoming asset or from other source

var loadConfig = new EntityLoadConfiguration(
    CultureLoadOption.Default, 
    new PropertyLoadOption(GetPropertyLoadOptions()), 
    new RelationLoadOption(GetRelationLoadOptions()));

if (!targetId.HasValue)
{
    MClient.Logger.Error($"TargetId: {targetId.Value}");
    return;
}
var asset = await MClient.Entities.GetAsync(targetId.Value, loadConfig);

if (asset == null)
{
    MClient.Logger.Error("Target entity not found.");
    return;
}

try
{
    // Any properties that require updating need to be listed in GetPropertyLoadOptions() along with their relations if need GetRelationLoadOptions()
    asset.SetPropertyValue("Title", "<title>"); // set new property - Title
    asset.SetPropertyValue("FileName", "<fileName>"); // set new property - FileName

    await MClient.Entities.SaveAsync(asset).ConfigureAwait(false);
    
    MClient.Logger.Info($"Entity: {targetId.Value} has been updated.");
}
catch (Exception e)
{
    MClient.Logger.Error($"Unable to UPDATE new {ENTITY_NAME_TO_UPDATE} entity. Error: {e.Message} Stack: {e.StackTrace}");
    return;
}

// Property load options for asset/entity that was just created
// Update with property names that exist on incoming entity
string[] GetPropertyLoadOptions()
{
    return new string[] { "Title", "FileName" };  
}

// Relation load options for asset/entity that was just created
// Update with relation names that exist on incoming entity
string[] GetRelationLoadOptions()
{
    return new string[] { "TagToAsset" };
}