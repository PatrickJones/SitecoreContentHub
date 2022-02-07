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

const string ENTITY_NAME_TO_CREATE = "PJ.Lion"; // Update with entity name that is getting created

try
{
    var pjLionEntity = await MClient.EntityFactory.CreateAsync(ENTITY_NAME_TO_CREATE).ConfigureAwait(false);
    
    //*** Uncomment below and set properties on new entity
    //pjLionEntity.SetPropertyValue("FileName", fileName);

    var entityId = await MClient.Entities.SaveAsync(pjLionEntity).ConfigureAwait(false);
    if (entityId <= 0)
    {
        MClient.Logger.Error($"{ENTITY_NAME_TO_CREATE} entity not created.");
        return;
    }

    MClient.Logger.Info($"{ENTITY_NAME_TO_CREATE} id: {entiyId}");
}
catch (Exception e)
{
    MClient.Logger.Error($"Unable create new {ENTITY_NAME_TO_CREATE} entity. Error: {e.Message} Stack: {e.StackTrace}");
    return;
}
