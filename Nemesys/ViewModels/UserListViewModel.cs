using Microsoft.EntityFrameworkCore;
using Nemesys.Models;
using System.Collections.Generic;

namespace Nemesys.ViewModels
{
    [Keyless]
    public class UserListViewModel
    {
        public int TotalEntries { get; set; }
        public IEnumerable<AppUser> Users { get; set; }
    }
}