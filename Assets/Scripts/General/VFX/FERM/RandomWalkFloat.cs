using UnityEngine;

namespace KitTraden.Abeyance.VFX.FERM
{
    public class RandomWalkFloat
    {
        public float value { get; set; }
        public float speed { get; set; }
        public float maxSpeedDelta { get; set; }
        public float minValue { get; set; }
        public float maxValue { get; set; }
        public float minSpeed { get; set; }
        public float maxSpeed { get; set; }

        public RandomWalkFloat(
            float initialValue,
            float initialSpeed,
            float maxSpeedDelta,
            float minValue,
            float maxValue,
            float minSpeed,
            float maxSpeed
        )
        {
            this.value = initialValue;
            this.speed = initialSpeed;
            this.maxSpeedDelta = maxSpeedDelta;
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.minSpeed = minSpeed;
            this.maxSpeed = maxSpeed;
        }

        public RandomWalkFloat Clone()
        {
            return new RandomWalkFloat(
                value,
                speed,
                maxSpeedDelta,
                minValue,
                maxValue,
                minSpeed,
                maxSpeed
            );
        }

        public float Update(float deltaTime)
        {
            float speedDelta = Random.Range(-maxSpeedDelta, maxSpeedDelta) * deltaTime;
            float newSpeed = speed + speedDelta;

            // Prevent speed from getting to an extreme and then staying there for an
            // extended period of time
            if (newSpeed < minSpeed | newSpeed > maxSpeed)
            {
                newSpeed = Mathf.Clamp(speed - speedDelta, minValue, maxValue);
            }

            float newValue = value + newSpeed;

            // When value is at one of its extremes, it's a strong indicator that
            // speed is highly negative or positive. So wrapping around to the other
            // extreme keeps things animated. Might lead to some jumpiness, but hey,
            // this is for the weird geometery, as long as it looks good!
            if (newValue < minValue || newValue > maxValue)
            {
                float distance = maxValue - minValue;
                if (newValue < minValue)
                {
                    newValue += distance;
                }
                else
                {
                    newValue -= distance;
                }

                newValue = Mathf.Clamp(newValue, minValue, maxValue);
            }

            speed = newSpeed;
            value = newValue;

            return value;
        }
    }
}