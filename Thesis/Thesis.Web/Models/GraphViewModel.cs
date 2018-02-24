using System.Collections.Generic;
using Domain.GraphClasses;
using Thesis.Web.DTOs;
using UserDto = Domain.DTOs.UserDto;

namespace Thesis.Web.Models
{
    public class GraphViewModel
    {
        public List<TeamMemberDto> TeamMembers { get; set; }
        public int? SelectedTeamMemberId { get; set; }
        public VisGraphViewModel visGraphViewModel { get; set; }

        public Graph<UserDto> Graph { get; set; }
        
    }
}