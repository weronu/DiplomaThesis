using System.Collections.Generic;
using Thesis.Web.DTOs;

namespace Thesis.Web.Models
{
    public class TeamMemberViewModel
    {
        public List<TeamMemberDto> TeamMembers { get; set; }
        public int? SelectedTeamMemberId { get; set; }
        public VisGraphViewModel visGraphViewModel { get; set; }
    }
}