﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BDAT1001.ContactManagement.Data;
using BDAT1001.ContactManagement.Models;
using BDAT1001.ContactManagement.Authorization;
using BDAT1001.ContactManagement.Pages.Contacts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BDAT1001.ContactManagement.Pages
{
    public class EditModel : DI_BasePageModel
    {
        public EditModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager)
            : base(context, authorizationService, userManager)
        {
        }

        [BindProperty]
        public Contact Contact { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Contact? contact = await Context.Contact.FirstOrDefaultAsync(
                                                             m => m.ContactId == id);
            if (contact == null)
            {
                return NotFound();
            }

            Contact = contact;

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                      User, Contact,
                                                      ContactOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Fetch Contact from DB to get OwnerID.
            var contact = await Context
                .Contact.AsNoTracking()
            .FirstOrDefaultAsync(m => m.ContactId == id);

            if (contact == null)
            {
                return NotFound();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                     User, contact,
                                                     ContactOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            Contact.OwnerID = contact.OwnerID;

            Context.Attach(Contact).State = EntityState.Modified;

            if (Contact.Status == ContactStatus.Approved)
            {
                // If the contact is updated after approval, 
                // and the user cannot approve,
                // set the status back to submitted so the update can be
                // checked and approved.
                var canApprove = await AuthorizationService.AuthorizeAsync(User,
                                        Contact,
                                        ContactOperations.Approve);

                if (!canApprove.Succeeded)
                {
                    Contact.Status = ContactStatus.Submitted;
                }
            }

            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}