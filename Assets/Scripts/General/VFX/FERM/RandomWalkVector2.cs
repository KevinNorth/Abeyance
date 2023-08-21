using UnityEngine;

namespace KitTraden.Abeyance.VFX.FERM
{
    public class RandomWalkVector2
    {
        private RandomWalkFloat xWalk;
        private RandomWalkFloat yWalk;

        public Vector2 value
        {
            get
            {
                return new Vector2(xWalk.value, yWalk.value);
            }
            set
            {
                xWalk.value = value.x;
                yWalk.value = value.y;
            }
        }

        public Vector2 speed
        {
            get
            {
                return new Vector2(xWalk.speed, yWalk.speed);
            }
            set
            {
                xWalk.speed = value.x;
                yWalk.speed = value.y;
            }
        }

        public Vector2 maxSpeedDelta
        {
            get
            {
                return new Vector2(xWalk.maxSpeedDelta, yWalk.maxSpeedDelta);
            }
            set
            {
                xWalk.maxSpeedDelta = value.x;
                yWalk.maxSpeedDelta = value.y;
            }
        }

        public Vector2 minValue
        {
            get
            {
                return new Vector2(xWalk.minValue, yWalk.minValue);
            }
            set
            {
                xWalk.minValue = value.x;
                yWalk.minValue = value.y;
            }
        }

        public Vector2 maxValue
        {
            get
            {
                return new Vector2(xWalk.maxValue, yWalk.maxValue);
            }
            set
            {
                xWalk.maxValue = value.x;
                yWalk.maxValue = value.y;
            }
        }

        public Vector2 minSpeed
        {
            get
            {
                return new Vector2(xWalk.minSpeed, yWalk.minSpeed);
            }
            set
            {
                xWalk.minSpeed = value.x;
                yWalk.minSpeed = value.y;
            }
        }

        public Vector2 maxSpeed
        {
            get
            {
                return new Vector2(xWalk.maxSpeed, yWalk.maxSpeed);
            }
            set
            {
                xWalk.maxSpeed = value.x;
                yWalk.maxSpeed = value.y;
            }
        }

        public RandomWalkVector2(
            Vector2 initialValue,
            Vector2 initialSpeed,
            Vector2 maxSpeedDelta,
            Vector2 minValue,
            Vector2 maxValue,
            Vector2 minSpeed,
            Vector2 maxSpeed
        )
        {
            xWalk = new RandomWalkFloat(0, 0, 0, 0, 0, 0, 0);
            yWalk = new RandomWalkFloat(0, 0, 0, 0, 0, 0, 0);

            this.value = initialValue;
            this.speed = initialSpeed;
            this.maxSpeedDelta = maxSpeedDelta;
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.minSpeed = minSpeed;
            this.maxSpeed = maxSpeed;
        }

        public RandomWalkVector2 Clone()
        {
            return new RandomWalkVector2(
                value,
                speed,
                maxSpeedDelta,
                minValue,
                maxValue,
                minSpeed,
                maxSpeed
            );
        }


        public Vector2 Update(float deltaTime)
        {
            return new Vector2(
                xWalk.Update(deltaTime),
                yWalk.Update(deltaTime)
            );
        }
    }
}