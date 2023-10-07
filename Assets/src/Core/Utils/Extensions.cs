using System;
using UnityEngine;
using UnityEngine.AI;

namespace Core.Utils
{
    public static class Extensions
    {
        /// <summary>
        /// // string to int (returns errVal if failed)
        /// </summary>
        /// <returns>Int Value</returns>
        /// <param name="value">string.</param>
        /// <param name="errVal">Default value</param>
        public static int ToInt(this string value, int errVal = 0)
        {
            Int32.TryParse(value, out errVal);
            return errVal;
        }

        /// <summary>
        /// string to long (returns errVal if failed)
        /// </summary>
        /// <returns>long Value</returns>
        /// <param name="value">string.</param>
        /// <param name="errVal">Default value</param>
        public static long ToLong(this string value, long errVal = 0)
        {
            Int64.TryParse(value, out errVal);
            return errVal;
        }

        /// <summary>
        /// returns the nearest valid point for a given destination.
        /// </summary>
        /// <returns>The valid destination.</returns>
        /// <param name="agent">Agent.</param>
        /// <param name="destination">Destination Vector3.</param>
        public static Vector3 NearestValidDestination(this NavMeshAgent agent, Vector3 destination, NavMeshPath path)
        {
            // can we calculate a path there? then return the closest valid point
            if (agent.CalculatePath(destination, path))
                return path.corners[path.corners.Length - 1];

            // otherwise find nearest navmesh position first. we use a radius of
            // speed*2 which works fine. afterwards we find the closest valid point.
            NavMeshHit hit;
            if (NavMesh.SamplePosition(destination, out hit, agent.speed * 2, NavMesh.AllAreas))
                if (agent.CalculatePath(hit.position, path))
                    return path.corners[path.corners.Length - 1];

            // nothing worked, don't go anywhere.
            return agent.transform.position;
        }

        public static Transform FindRecursively(this Transform transform, string name)
        {
            return Array.Find(transform.GetComponentsInChildren<Transform>(true),
                t => t.name == name);
        }

        public static Vector3 RandomNavSphere(this NavMeshAgent agent, Vector3 origin, float distance)
        {
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * distance;

            randomDirection += origin;

            NavMeshHit navHit;

            if (NavMesh.SamplePosition(randomDirection, out navHit, distance, NavMesh.AllAreas))
                return navHit.position;

            return origin;
        }
    }
}
