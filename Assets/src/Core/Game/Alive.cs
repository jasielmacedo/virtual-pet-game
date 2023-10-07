using UnityEngine;
using System.Collections;
using Core.Utils;

namespace Core.Game
{
  public class Alive : Actor
  {

    public bool isAlive
    {
      get
      {
        return (heart >= 1);
      }
    }

    [Header("Livable Settings")]
    [SerializeField] protected bool canReceiveDamage = true;
    [SerializeField] protected int maxHeart = 100;

    public int myMaxHeart
    {
      get
      {
        return this.maxHeart;
      }
    }

    public int myHeart
    {
      get
      {
        return this.heart;
      }
      set
      {
        if (value > this.maxHeart)
          this.heart = (Integering)this.maxHeart;
        else if (value < 0)
          this.heart = (Integering)0;
        else
          this.heart = (Integering)value;
      }
    }
    private Integering heart = (Integering)100;

    protected override void Awake()
    {
      base.Awake();
      this.heart = (Integering)this.maxHeart;
    }

    protected virtual void Start() { }


    public delegate void OnTakeAnyDamageHandle(int heartRemaining);
    public event OnTakeAnyDamageHandle OnTakeAnyDamage;

    public delegate void OnTakeDeathHandle(Alive who);
    public event OnTakeDeathHandle OnTakeDeath;

    public virtual void ApplyDamage(int damage, Alive damageCauser = null)
    {
      if (isAlive && canReceiveDamage)
      {
        this.heart -= damage;

        if (heart > 0)
        {
          if (OnTakeAnyDamage != null)
            OnTakeAnyDamage(this.heart);
        }
        else
        {
          if (OnTakeDeath != null)
            OnTakeDeath(this);

          heart = (Integering)0;
          this.getDeath();
        }
      }
    }

    public virtual void ApplyHealing(int heal)
    {
      if (isAlive && canReceiveDamage)
      {
        this.heart += heal;
        if (this.heart > this.maxHeart)
          this.heart = (Integering)this.maxHeart;
      }
    }

    public virtual void getDeath() { }
  }
}
