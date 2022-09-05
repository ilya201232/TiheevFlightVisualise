using UnityEngine;
public static class Utils
{
    public static Vector3 GetClosetAxisVector(Vector3 vector)
    {
        var normVector = vector.normalized;

        var result = Vector3.forward;
        var minDistance = Vector3.Distance(Vector3.forward, normVector);

        var newDist = Vector3.Distance(Vector3.back, normVector);
        if (newDist < minDistance)
        {
            result = Vector3.back;
            minDistance = newDist;
        }
        
        newDist = Vector3.Distance(Vector3.left, normVector);
        if (newDist < minDistance)
        {
            result = Vector3.left;
            minDistance = newDist;
        }
        
        newDist = Vector3.Distance(Vector3.right, normVector);
        if (newDist < minDistance)
        {
            result = Vector3.right;
            minDistance = newDist;
        }
        
        newDist = Vector3.Distance(Vector3.up, normVector);
        if (newDist < minDistance)
        {
            result = Vector3.up;
            minDistance = newDist;
        }
        
        newDist = Vector3.Distance(Vector3.down, normVector);
        if (newDist < minDistance)
        {
            result = Vector3.down;
        }

        return result;
    }
    
    /*
The MIT License (MIT)

Copyright (c) 2021 Ryan Vazquez

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

 */

    public static Vector3 CustomVectorScale(
        Vector3 vector,
        float positiveX, float negativeX,
        float positiveY, float negativeY,
        float positiveZ, float negativeZ)
    {

        var result = vector;
        
        switch (result.x)
        {
            case > 0:
                result.x *= positiveX;
                break;
            case < 0:
                result.x *= negativeX;
                break;
        }
        
        switch (result.y)
        {
            case > 0:
                result.y *= positiveY;
                break;
            case < 0:
                result.y *= negativeY;
                break;
        }
        
        switch (result.z)
        {
            case > 0:
                result.z *= positiveZ;
                break;
            case < 0:
                result.z *= negativeZ;
                break;
        }

        return result;
    }
}