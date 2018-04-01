﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Domain.DTOs;
using Domain.GraphClasses;
using Thesis.Web.DTOs;


namespace Thesis.Web.Models
{
    public class GraphViewModel
    {
        public bool FileImported { get; set; }
        public List<TeamMemberDto> TeamMembers { get; set; }
        public int? SelectedTeamMemberId { get; set; }
        public GraphDto GraphDto { get; set; }
        public Graph<UserDto> Graph { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        
    }
}