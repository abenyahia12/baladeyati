using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using UnityEngine;

public class test : MonoBehaviour
{


	public static long angryAnimals(int n, List<int> a, List<int> b)
	{
		int m = a.Count;
		long result = n * (n + 1) / 2;
		int[] min = new int[m];
		int[] max = new int[m];
		for (int i = 0; i < m; i++)
		{
			if (a[i] > b[i])
			{
				min[i] = b[i];
				max[i] = a[i];
			}
			else
			{
				min[i] = a[i];
				max[i] = b[i];
			}
		}
		List<int> excluded = new List<int>();

		for (int i = 0; i < m; ++i)
		{
			bool found = false;

			for (int k = excluded.Count - 1; k >= 0; --k)
			{
				int j = excluded[k];
				if (min[j] >= min[i] && max[j] <= max[i])
				{
					found = true;
					break;
				}
				else if (min[i] >= min[j] && max[i] <= max[j])
				{
					excluded.RemoveAt(k);
				}
			}

			if (!found)
			{
				excluded.Add(i);
			}
		}

		excluded.Sort(delegate (int x, int y)
		{
			if (min[x] == min[y]) return 0;
			else if (min[x] < min[y]) return -1;
			else return 1;
		});

		int previousMin = 0;
		excluded.ForEach(delegate (int i)
		{
			result -= (min[i] - previousMin) * (n - max[i] + 1);
			previousMin = min[i];
		});


		return result;

	}
}