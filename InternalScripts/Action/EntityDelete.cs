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

if (!targetId.HasValue)
{
    MClient.Logger.Error("TargetId: " + targetId);
    return;
}

try
{
    await MClient.Entities.DeleteAsync(targetId).ConfigureAwait(false);
    
    MClient.Logger.Info(logData);
}
catch (Exception e)
{
    MClient.Logger.Error($"Unable to DELETE entity with id: {targetId} entity. Error: {e.Message} Stack: {e.StackTrace}");
    return;
}
