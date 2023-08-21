using UnityEngine;

namespace KitTraden.Abeyance.VFX.FERM
{
    public class RandomWalkVector3
    {
        private RandomWalkFloat xWalk;
        private RandomWalkFloat yWalk;
        private RandomWalkFloat zWalk;

        public Vector3 value
        {
            get
            {
                return new Vector3(xWalk.value, yWalk.value, zWalk.value);
            }
            set
            {
                xWalk.value = value.x;
                yWalk.value = value.y;
                zWalk.value = value.z;
            }
        }

        public Vector3 speed
        {
            get
            {
                return new Vector3(xWalk.speed, yWalk.speed, zWalk.speed);
            }
            set
            {
                xWalk.speed = value.x;
                yWalk.speed = value.y;
                zWalk.speed = value.z;
            }
        }

        public Vector3 maxSpeedDelta
        {
            get
            {
                return new Vector3(xWalk.maxSpeedDelta, yWalk.maxSpeedDelta, zWalk.maxSpeedDelta);
            }
            set
            {
                xWalk.maxSpeedDelta = value.x;
                yWalk.maxSpeedDelta = value.y;
                zWalk.maxSpeedDelta = value.z;
            }
        }

        public Vector3 minValue
        {
            get
            {
                return new Vector3(xWalk.minValue, yWalk.minValue, zWalk.minValue);
            }
            set
            {
                xWalk.minValue = value.x;
                yWalk.minValue = value.y;
                zWalk.minValue = value.z;
            }
        }

        public Vector3 maxValue
        {
            get
            {
                return new Vector3(xWalk.maxValue, yWalk.maxValue, zWalk.maxValue);
            }
            set
            {
                xWalk.maxValue = value.x;
                yWalk.maxValue = value.y;
                zWalk.maxValue = value.z;
            }
        }

        public Vector3 minSpeed
        {
            get
            {
                return new Vector3(xWalk.minSpeed, yWalk.minSpeed, zWalk.minSpeed);
            }
            set
            {
                xWalk.minSpeed = value.x;
                yWalk.minSpeed = value.y;
                zWalk.minSpeed = value.z;
            }
        }

        public Vector3 maxSpeed
        {
            get
            {
                return new Vector3(xWalk.maxSpeed, yWalk.maxSpeed, zWalk.maxSpeed);
            }
            set
            {
                xWalk.maxSpeed = value.x;
                yWalk.maxSpeed = value.y;
                zWalk.maxSpeed = value.z;
            }
        }

        public RandomWalkVector3(
            Vector3 initialValue,
            Vector3 initialSpeed,
            Vector3 maxSpeedDelta,
            Vector3 minValue,
            Vector3 maxValue,
            Vector3 minSpeed,
            Vector3 maxSpeed
        )
        {
            xWalk = new RandomWalkFloat(0, 0, 0, 0, 0, 0, 0);
            yWalk = new RandomWalkFloat(0, 0, 0, 0, 0, 0, 0);
            zWalk = new RandomWalkFloat(0, 0, 0, 0, 0, 0, 0);

            this.value = initialValue;
            this.speed = initialSpeed;
            this.maxSpeedDelta = maxSpeedDelta;
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.minSpeed = minSpeed;
            this.maxSpeed = maxSpeed;
        }

        public RandomWalkVector3 Clone()
        {
            return new RandomWalkVector3(
                value,
                speed,
                maxSpeedDelta,
                minValue,
                maxValue,
                minSpeed,
                maxSpeed
            );
        }

        public Vector3 Update(float deltaTime)
        {
            return new Vector3(
                xWalk.Update(deltaTime),
                yWalk.Update(deltaTime),
                zWalk.Update(deltaTime)
            );
        }
    }

}