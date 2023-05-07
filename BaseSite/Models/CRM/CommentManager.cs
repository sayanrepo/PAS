using BaseSite.Data;
using BaseSite.Models.DBModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace BaseSite.Models.CRM
{
    public class CommentManager
    {
        public static List<CRM_Comments> CRM_Comments_Get(short trunktableId, int trunkId)
        {
            using (var context = new PantaEntities())
            {
                List<CRM_Comments> res = context.CRM_Comments.Include(m => m.Account_Users)
                    .Include(m => m.CRM_Comments1).Include(m => m.CRM_Comments1.Select(n => n.Account_Users))
                    .Where(m => m.TrunkTableId == trunktableId && m.TrunkId == trunkId && m.ParentId == null).ToList();
                return res;
            }
        }

        public static CRM_Comments CRM_Comments_Get(int id)
        {
            using (var context = new PantaEntities())
            {
                CRM_Comments res = context.CRM_Comments.Include(m => m.Account_Users)
                    .Include(m => m.CRM_Comments1).Include(m => m.CRM_Comments1.Select(n => n.Account_Users))
                    .Where(m => m.Id == id).SingleOrDefault();
                return res;
            }
        }

        public static CRM_Comments CRM_Comments_Edit(CRM_Comments comment)
        {
            using (var context = new PantaEntities())
            {
                if (comment.Id == 0)
                {
                    CRM_Comments newComment = new CRM_Comments();
                    newComment.TrunkId = comment.TrunkId;
                    newComment.TrunkTableId = comment.TrunkTableId;
                    newComment.ParentId = comment.ParentId;
                    newComment.OwnerId = comment.OwnerId;
                    newComment.OwnerName = comment.OwnerName;
                    newComment.OwnerEmail = comment.OwnerEmail;
                    newComment.Comment = comment.Comment;
                    newComment.CreateDate = DateTime.Now;

                    context.CRM_Comments.Add(newComment);
                    context.SaveChanges();
                    return CommentManager.CRM_Comments_Get(newComment.Id);
                }
                else
                {
                    CRM_Comments newComment = context.CRM_Comments.Include(m => m.Account_Users)
                    .Include(m => m.CRM_Comments1).Include(m => m.CRM_Comments1.Select(n => n.Account_Users))
                    .Where(m => m.Id == comment.Id).SingleOrDefault();

                    newComment.Comment = comment.Comment;

                    context.SaveChanges();
                    return CommentManager.CRM_Comments_Get(newComment.Id);
                }
            }
        }
    }
}