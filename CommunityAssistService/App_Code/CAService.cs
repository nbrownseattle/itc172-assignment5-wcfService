using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "CAService" in code, svc and config file together.
public class CAService : ICAService//shortcut:hover over name, click lightbulb to implement interface
{
    Community_AssistEntities cae = new Community_AssistEntities();//connecting to the ado objects

    public bool ApplyForGrant(GrantRequest gr)
    {
        bool result = true;
        try
        {
            GrantReview review = new global::GrantReview();
           // review.GrantRequest = gr;
           // review.GrantRequestStatus = "pending";
            cae.GrantRequests.Add(gr);
           // cae.GrantReviews.Add(review);
            cae.SaveChanges();
        }

        catch (Exception ex)
        {

            result = false;
            throw ex;
        }

        return result;

    }
    public List<GrantInfo> GetGrantsByPerson(int PersonId)
    {
        var grants = from g in cae.GrantRequests
                     from r in g.GrantReviews
                     where g.PersonKey == PersonId
                     select new
                     {
                         g.GrantRequestDate,
                         g.GrantRequestExplanation,
                         g.GrantType.GrantTypeName,
                         g.GrantRequestAmount,
                         r.GrantRequestStatus
                     };
        List<GrantInfo> info = new List<GrantInfo>();
        foreach (var gr in grants)
        {
            GrantInfo gi = new GrantInfo();
            gi.GrantTypeName = gr.GrantTypeName;
            gi.Explanation = gr.GrantRequestExplanation;
            gi.Amount = (decimal)gr.GrantRequestAmount;
            gi.Status = gr.GrantRequestStatus;

            info.Add(gi);
        }
        return info;
    }

    public List<GrantType> GetGrantTypes()
    {
        var types = from g in cae.GrantTypes
                     select new//the query
                     {
                         g.GrantTypeKey,
                         g.GrantTypeName
                     };
        List<GrantType> gTypes = new List<GrantType>();//creates the object
        foreach(var t in types)//loops through the query results, adds objects to our list
        {
            GrantType   type = new GrantType();
            type.GrantTypeKey = t.GrantTypeKey;
            type.GrantTypeName = t.GrantTypeName;
            gTypes.Add(type);
        }
        return gTypes;//returns the list
    } 

    public int PersonLogin(string user, string password)
    {   
        //password test
        int key = 0;
        int result = cae.usp_Login(user, password);
        if(result != -1)
        {
            //query to get person key
            var ukey = (from p in cae.People
                        where p.PersonEmail.Equals(user)
                        select p.PersonKey).FirstOrDefault();
            key = (int)ukey;
        }
        return key;//return the person key
    }

    public bool RegisterPerson(PersonLite p)
    {
        bool result = true;
        int success = cae.usp_Register(p.LastName, p.FirstName, p.Email,
            p.Password, p.Apartment, p.City, p.Street, p.State,
            p.Zipcode, p.HomePhone, p.WorkPhone);
        if (success == -1)//if failed
        {
            result = false;
        }

        return result;
       
    }
}
