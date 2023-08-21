using UnityEngine;

namespace KitTraden.Abeyance.VFX.FERM
{
    public class RandomWalkQuaternion
    {
        private RandomWalkFloat xWalk;
        private RandomWalkFloat yWalk;
        private RandomWalkFloat zWalk;
        private RandomWalkFloat wWalk;

        public Quaternion value
        {
            get
            {
                return new Quaternion(xWalk.value, yWalk.value, zWalk.value, wWalk.value);
            }
            set
            {
                xWalk.value = value.x;
                yWalk.value = value.y;
                zWalk.value = value.z;
                wWalk.value = value.w;
            }
        }

        public Vector4 speed
        {
            get
            {
                return new Vector4(xWalk.speed, yWalk.speed, zWalk.speed, wWalk.speed);
            }
            set
            {
                xWalk.speed = value.x;
                yWalk.speed = value.y;
                zWalk.speed = value.z;
                wWalk.speed = value.w;
            }
        }

        public Vector4 maxSpeedDelta
        {
            get
            {
                return new Vector4(
                    xWalk.maxSpeedDelta,
                    yWalk.maxSpeedDelta,
                    zWalk.maxSpeedDelta,
                    wWalk.maxSpeedDelta
                );
            }
            set
            {
                xWalk.maxSpeedDelta = value.x;
                yWalk.maxSpeedDelta = value.y;
                zWalk.maxSpeedDelta = value.z;
                wWalk.maxSpeedDelta = value.w;
            }
        }

        public Vector4 minValue
        {
            get
            {
                return new Vector4(
                    xWalk.minValue,
                    yWalk.minValue,
                    zWalk.minValue,
                    wWalk.minValue
                );
            }
            set
            {
                xWalk.minValue = value.x;
                yWalk.minValue = value.y;
                zWalk.minValue = value.z;
                wWalk.minValue = value.w;
            }
        }

        public Vector4 maxValue
        {
            get
            {
                return new Vector4(
                    xWalk.maxValue,
                    yWalk.maxValue,
                    zWalk.maxValue,
                    wWalk.maxValue
                );
            }
            set
            {
                xWalk.maxValue = value.x;
                yWalk.maxValue = value.y;
                zWalk.maxValue = value.z;
                wWalk.maxValue = value.w;
            }
        }

        public Vector4 minSpeed
        {
            get
            {
                return new Vector4(
                    xWalk.minSpeed,
                    yWalk.minSpeed,
                    zWalk.minSpeed,
                    wWalk.minSpeed
                );
            }
            set
            {
                xWalk.minSpeed = value.x;
                yWalk.minSpeed = value.y;
                zWalk.minSpeed = value.z;
                wWalk.minSpeed = value.w;
            }
        }

        public Vector4 maxSpeed
        {
            get
            {
                return new Vector4(
                    xWalk.maxSpeed,
                    yWalk.maxSpeed,
                    zWalk.maxSpeed,
                    wWalk.maxSpeed
                );
            }
            set
            {
                xWalk.maxSpeed = value.x;
                yWalk.maxSpeed = value.y;
                zWalk.maxSpeed = value.z;
                wWalk.maxSpeed = value.w;
            }
        }

        public RandomWalkQuaternion(
            Quaternion initialValue,
            Vector4 initialSpeed,
            Vector4 maxSpeedDelta,
            Vector4 minValue,
            Vector4 maxValue,
            Vector4 minSpeed,
            Vector4 maxSpeed
        )
        {
            xWalk = new RandomWalkFloat(0, 0, 0, 0, 0, 0, 0);
            yWalk = new RandomWalkFloat(0, 0, 0, 0, 0, 0, 0);
            zWalk = new RandomWalkFloat(0, 0, 0, 0, 0, 0, 0);
            wWalk = new RandomWalkFloat(0, 0, 0, 0, 0, 0, 0);

            this.value = initialValue;
            this.speed = initialSpeed;
            this.maxSpeedDelta = maxSpeedDelta;
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.minSpeed = minSpeed;
            this.maxSpeed = maxSpeed;
        }

        public RandomWalkQuaternion Clone()
        {
            return new RandomWalkQuaternion(
                value,
                speed,
                maxSpeedDelta,
                minValue,
                maxValue,
                minSpeed,
                maxSpeed
            );
        }

        public Quaternion Update(float deltaTime)
        {
            return new Quaternion(
                xWalk.Update(deltaTime),
                yWalk.Update(deltaTime),
                zWalk.Update(deltaTime),
                wWalk.Update(deltaTime)
            );
        }
    }

}