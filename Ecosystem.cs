public class LifeForm
{
    public int LifePoints { get; set; }
    public int EnergyReserves { get; set; }
    public int X { get; set; }
    public int Y { get; set; }

    public void Update()
    {
        EnergyReserves--;
        if (EnergyReserves <= 0)
        {
            EnergyReserves += LifePoints;
            LifePoints = 0;
        }
    }

    public void Eat(LifeForm food)
    {
        EnergyReserves += food.LifePoints;
        food.LifePoints = 0;
    }

    public void Reproduce(LifeForm partner)
    {
        int gestationPeriod = 10; // Number of updates before birth
        int youngLifePoints = 10; // Life points of newborn

        if (partner is Animal)
        {
            // Male and female must make contact
            if (IsInContactZone(partner))
            {
                // Gestation period
                for (int i = 0; i < gestationPeriod; i++)
                {
                    Update();
                }

                // Birth of young
                var young = new Animal();
                young.LifePoints = youngLifePoints;
                young.EnergyReserves = EnergyReserves / 2; // Young inherits half of parent's energy reserves
                LifeForms.Add(young);
            }
        }
    }

    private double GetDistance(LifeForm target)
    {
        // Calculate distance between this life form and the target
        int dx = target.X - X;
        int dy = target.Y - Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }
}

public class Animal : LifeForm
{
    public bool IsCarnivore { get; set; }
    public int VisionZone { get; set; }
    public int ContactZone { get; set; }

    public void Move()
    {
        // Movement logic
        int moveDistance = 5; // Distance to move in a single update
        int moveDirection = 0; // Direction to move in degrees (0-360)

        // Calculate new position based on moveDistance and moveDirection
        double radians = moveDirection * Math.PI / 180;
        int newX = (int)(X + moveDistance * Math.Cos(radians));
        int newY = (int)(Y + moveDistance * Math.Sin(radians));

        // Update position
        X = newX;
        Y = newY;
    }

    public void Attack(LifeForm target)
    {
        if (IsCarnivore && IsInContactZone(target))
        {
            target.LifePoints = 0;
        }
    }

    private bool IsInContactZone(LifeForm target)
    {
        return GetDistance(target) <= ContactZone;
    }
}

public class Plant : LifeForm
{
    public int RootZone { get; set; }
    public int SeedZone { get; set; }

    public void Spread()
    {
        // Create new plants in seed zone
        List<LifeForm> newPlants = new List<LifeForm>();
        foreach (var lifeForm in LifeForms)
        {
            if (lifeForm is Plant && GetDistance(lifeForm) <= SeedZone)
            {
                Plant plant = (Plant)lifeForm;
                Plant newPlant = new Plant();
                newPlant.LifePoints = plant.LifePoints / 2; // New plant has half of parent's life points
                newPlant.EnergyReserves = plant.EnergyReserves / 2; // New plant has half of parent's energy reserves
                newPlants.Add(newPlant);
            }
        }
        LifeForms.AddRange(newPlants);
    }
}

public class OrganicWaste : LifeForm
{
    public int DecayTime { get; set; }

    public void Update()
    {
        DecayTime--;
        if (DecayTime <= 0)
        {
            LifePoints = 0;
        }
    }
}

public class World
{
    public List<LifeForm> LifeForms { get; set; }

    public void Update()
    {
        // Update all life forms
        foreach (var lifeForm in LifeForms)
        {
            lifeForm.Update();
        }

        // Remove dead life forms
        LifeForms = LifeForms.Where(x => x.LifePoints > 0).ToList();
    }
}