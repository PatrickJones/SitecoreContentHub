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
const string SAFARI_RELATION_NAME = "SafariToAsset"; // Update with entity name that is getting created
const string ENTITY_RELATION_NAME = "PJ.Safari.Samburu"; // Update with entity name that is getting created
const string ENTITY_RELATION_SAMBURU_PARENT = "PJ.Safari.Destination"; // Update with entity name that is getting created
const string ENTITY_RELATION_DESTINATION_PARENT = "PJ.Safari.Kenya"; // Update with entity name that is getting created
const string ENTITY_RELATION_KENYA_PARENT = "PJ.Safari.Location"; // Update with entity name that is getting created

const bool CHAIN_RELATIONSHIP = true; //true - Chains the relationships in the entity; false - Adds only single relationship

try
{
    var pjLionEntity = await MClient.EntityFactory.CreateAsync(ENTITY_NAME_TO_CREATE).ConfigureAwait(false);
    
    //*** Uncomment below and set properties on new entity
    pjLionEntity.SetPropertyValue("Name", "Relationship Test");

    if (CHAIN_RELATIONSHIP)
    {
        var relationshipIdentifiers = new string[] { ENTITY_RELATION_NAME, ENTITY_RELATION_SAMBURU_PARENT, ENTITY_RELATION_DESTINATION_PARENT, ENTITY_RELATION_KENYA_PARENT};
        var relationshipEntities = await MClient.Entities.GetManyAsync(relationshipIdentifiers);

        var samburuEntity = relationshipEntities.Where(w => w.Identifier == ENTITY_RELATION_NAME).First();
        var destinationEntity = relationshipEntities.Where(w => w.Identifier == ENTITY_RELATION_SAMBURU_PARENT).First();
        var kenyaEntity = relationshipEntities.Where(w => w.Identifier == ENTITY_RELATION_DESTINATION_PARENT).First();
        var locationEntity = relationshipEntities.Where(w => w.Identifier == ENTITY_RELATION_KENYA_PARENT).First();

        var safariRelation = pjLionEntity.GetRelation<IChildToManyParentsRelation>(SAFARI_RELATION_NAME);
        
        safariRelation.Parents.Add(locationEntity.Id.Value);
        safariRelation.Parents.Add(kenyaEntity.Id.Value);
        safariRelation.Parents.Add(destinationEntity.Id.Value);
        safariRelation.Parents.Add(samburuEntity.Id.Value);
    }
    else
    {
        var safariEntity = await MClient.Entities.GetAsync(ENTITY_RELATION_NAME).ConfigureAwait(false);
        if (safariEntity == null)
        {
            MClient.Logger.Error($"{ENTITY_RELATION_NAME} related entity not found.");
            return;        
        }

        var safariRelation = pjLionEntity.GetRelation<IChildToManyParentsRelation>(SAFARI_RELATION_NAME);
        
        MClient.Logger.Info($"{ENTITY_RELATION_NAME} id: {safariEntity.Id.Value}");
        
        safariRelation.Parents.Add(safariEntity.Id.Value);
    }
    
    var entityId = await MClient.Entities.SaveAsync(pjLionEntity).ConfigureAwait(false);
    if (entityId <= 0)
    {
        MClient.Logger.Error($"{ENTITY_NAME_TO_CREATE} entity not created.");
        return;
    }

    MClient.Logger.Info($"{ENTITY_NAME_TO_CREATE} id: {entityId}");
}
catch (Exception e)
{
    MClient.Logger.Error($"Unable create new {ENTITY_NAME_TO_CREATE} entity. Error: {e.Message} Stack: {e.StackTrace}");
    return;
}
