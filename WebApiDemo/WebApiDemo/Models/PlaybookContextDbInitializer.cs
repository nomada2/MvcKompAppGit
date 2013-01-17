﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace WebApiDemo.Models
{
    public class PlaybookContextDbInitializer : DropCreateDatabaseIfModelChanges<PlaybookContext>
    {
        protected override void Seed(PlaybookContext context)
        {
            var tag1 = new Tag { Name = "Ball Handling" };
            var tag2 = new Tag { Name = "Passing" };
            var tag3 = new Tag { Name = "Shooting" };
            var tag4 = new Tag { Name = "Rebounding" };
            var tag5 = new Tag { Name = "Transition" };
            var tag6 = new Tag { Name = "Team Offense" };
            var tag7 = new Tag { Name = "Team Defense" };

            context.Tags.Add(tag1);
            context.Tags.Add(tag2);
            context.Tags.Add(tag3);
            context.Tags.Add(tag4);
            context.Tags.Add(tag5);
            context.Tags.Add(tag6);
            context.Tags.Add(tag7);

            context.SaveChanges();

            // ball handling
            context.Drills.Add(new Drill { Name = "Alley Drills", Description = null, Tag = tag1 });
            context.Drills.Add(new Drill { Name = "Machine Gun", Description = null, Tag = tag1 });
            context.Drills.Add(new Drill { Name = "Scissors dribble", Description = null, Tag = tag1 });
            context.Drills.Add(new Drill { Name = "Figure 8", Description = null, Tag = tag1 });

            // passing
            context.Drills.Add(new Drill { Name = "Perfection", Description = null, Tag = tag2 });
            context.Drills.Add(new Drill { Name = "3-man Weave", Description = null, Tag = tag2 });
            context.Drills.Add(new Drill { Name = "3-line full-court passing", Description = null, Tag = tag2 });

            // shooting
            context.Drills.Add(new Drill { Name = "Form Shooting", Description = null, Tag = tag3 });
            context.Drills.Add(new Drill { Name = "Elbow Shooting", Description = null, Tag = tag3 });
            context.Drills.Add(new Drill { Name = "Catch & Shoot", Description = null, Tag = tag3 });

            // rebounding
            context.Drills.Add(new Drill { Name = "War Drill", Tag = tag4 });
            context.Drills.Add(new Drill { Name = "Box-out drill", Tag = tag4 });

            // transition
            context.Drills.Add(new Drill { Name = "3-on-2, 2-on-1", Description = null, Tag = tag5 });
            context.Drills.Add(new Drill { Name = "3-on-2 Continuous", Description = null, Tag = tag5 });

            // team offense
            context.Drills.Add(new Drill { Name = "Motion", Description = null, Tag = tag6 });
            context.Drills.Add(new Drill { Name = "Flex", Description = null, Tag = tag6 });

            // team defense
            context.Drills.Add(new Drill { Name = "Red", Description = "1-2-1-1 Full court press", Tag = tag7 });
            context.Drills.Add(new Drill { Name = "23", Description = "2-3 zone", Tag = tag7 });
            context.Drills.Add(new Drill { Name = "13", Description = "1-3-1 zone", Tag = tag7 });
            context.Drills.Add(new Drill { Name = "1", Description = "Man-to-Man", Tag = tag7 });

            context.SaveChanges();
        }
    }
}