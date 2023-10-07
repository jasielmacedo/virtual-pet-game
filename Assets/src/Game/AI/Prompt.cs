using System.Collections.Generic;
using System.Text;

namespace AI
{
    public class Prompt
    {
        public static string CreatoSystemPrompt(string companyName, string companyGoal)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("You are in a game simulation and you are the CreatorGPT, an AI designed to propose agents based on the user's input.\n");
            sb.Append($"The company name is {companyName} and the company goal is {companyGoal}");

            PromptGenerator directives = new PromptGenerator();

            directives.AddGoal("Propose agents to acomplish the company goal.");
            directives.AddGoal("Give agents names, role and personal goals in the company");
            directives.AddGoal("Create up to 5 agents");

            directives.AddConstraints("You should follow the format specified below (no other text is necessary)");
            directives.AddConstraints("Do not seek for user assistance");

            directives.SetJsonFormat("{ name: 'Name of the employee', role: 'Role in the company', gender: 'male/female', 'goal': 'personal goal for seek the company goal' }");

            sb.Append(directives.ToString());

            return sb.ToString();
        }

        public class PromptGenerator
        {
            public List<string> goals = new List<string>();
            public List<string> constraints = new List<string>();
            public List<string> resources = new List<string>();
            public List<string> performanceEvaluation = new List<string>();
            public string jsonFormat;

            public void SetJsonFormat(string jsonFormat)
            {
                this.jsonFormat = jsonFormat;
            }

            public void AddGoal(string constraint)
            {
                this.goals.Add(constraint);
            }

            public void AddConstraints(string constraint)
            {
                this.constraints.Add(constraint);
            }

            public void AddResources(string resource)
            {
                this.resources.Add(resource);
            }

            public void AddPerformanceEvaluation(string evaluation)
            {
                this.performanceEvaluation.Add(evaluation);
            }

            private string generateNumberedList(List<string> list)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < list.Count; i++)
                {
                    sb.Append($"{i + 1}. {list[i]}");
                    sb.Append(i < list.Count - 1 ? "\n" : "");
                }
                return sb.ToString();
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                if (this.goals.Count > 0)
                    sb.Append($"GOALS:\n{this.generateNumberedList(this.goals)}\n\n");
                if (this.constraints.Count > 0)
                    sb.Append($"CONSTRAINTS:\n{this.generateNumberedList(this.constraints)}\n\n");
                if (this.resources.Count > 0)
                    sb.Append($"Resources:\n{this.generateNumberedList(this.resources)}\n\n");
                if (this.performanceEvaluation.Count > 0)
                    sb.Append($"Performance Evaluation:\n{this.generateNumberedList(this.performanceEvaluation)}\n\n");
                sb.Append("You should only respond in JSON format as described below\n");
                sb.Append($"Response Format:\n{jsonFormat}\nEnsure the response can be parsed by Python json.loads");

                return sb.ToString();
            }
        }
    }
}