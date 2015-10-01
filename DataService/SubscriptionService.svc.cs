﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DataService.Interfaces;
using DataService.Types;


namespace DataService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "SubscriptionService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select SubscriptionService.svc or SubscriptionService.svc.cs at the Solution Explorer and start debugging.
    public class SubscriptionService : ISubscriptionService
    {
        public IEnumerable<ApiSubscription> GetSubscriptions()
        {
            using (serviceEntities container = new serviceEntities())
            {
                foreach (var sub in container.Subscriptions)
                {
                    yield return new ApiSubscription(sub);
                }
            }
        }

        public IEnumerable<ApiSubscription> GetUserSubscriptions(int userId)
        {
            using (serviceEntities container = new serviceEntities())
            {
                var user = container.Users.Find(userId);
                if (user == null)
                {
                    throw new FaultException("No such user");
                }
                foreach (var sub in user.Subscriptions)
                {
                    yield return new ApiSubscription(sub);
                }
            }
        }

        public void DeleteSubscription(Guid subscriptionId)
        {
            using (serviceEntities container = new serviceEntities())
            {
                try
                {
                    var sub = container.Subscriptions.Find(subscriptionId);
                    container.Subscriptions.Remove(sub);
                    container.SaveChanges();
                }
                catch (Exception)
                {
                    //todo add logging  
                }//if its there its already deleted 
            }
        }

        public ApiSubscription UpdateSubscription(ApiSubscription subValues)
        {
            using (serviceEntities container = new serviceEntities())
            {
                var sub = container.Subscriptions.Find(subValues.Id);
                if (sub == null)
                {
                    throw new FaultException("No such subscription");
                }
                sub.Name = subValues.Name;
                sub.Price = subValues.Price;
                sub.PriceIncVatAmount = subValues.PriceIncVatAmount;
                sub.UrlFriendly = Utilities.toUrlFriendlyIndentifier(subValues.Name);
                container.SaveChanges();
                return new ApiSubscription(sub);
            }
        }
    }
}
