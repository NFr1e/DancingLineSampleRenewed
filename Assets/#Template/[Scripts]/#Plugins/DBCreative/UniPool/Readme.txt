UniPool 
User Manual

Add UniPool prefab to your scene and you are ready to start pooling.

There may be a bit of text below(for more in-depth information), but it's really straightforward to use... 
Essentially, you replace Instantiate, Destroy, GetComponent<T>(), .transform, .gameObject with UniPool's alternatives
like: Get, Release, GetComponent<T>(), and cached .transform, .gameObject
Additionally, you may specify separate containers for pooled GameObjects to reside while inactive.

(Make sure to read "Pooling Quirks" at the end of the manual, especially if you are pooling UGUI objects)

The PoolManager (main feature class of UniPool) allows to pool all kinds of prefab GameObject all in the same pool.
PoolCell is the pool entry that caches GameObject, Transform.
Additionally, can also cache ONE Component of your choice, on the instantiated GameObject or its children.
This provides a significant performance increase, by allowing to avoid external GetComponent calls.
Note: use cached objects instead of
 gameObject.transform
 transform.gameObject
 Monobehaviour.gameObject
 Monobehaviour.transform
 GetComponent<T>();
These calls significantly degrade performance as scale grows. Instead, use the PoolCell's versions.

----
Use Info / Examples

Assume:
GameObject TEST_Prefab - is prefab
string TEST_Prefab_name - is pool name for the prefab

Output of PoolManager.Get is the PoolCell, which contains cached GameObject, Transform, 
and if specified one Component of interest.
You should use these cached objects instead of getting them manually, 
this will significantly increase performance.

In Short:

If you don't need to cache a component, you can use one of these ways to pool/get:

Dynamic without registering:

PoolCell cell = UniPool.Get(TEST_Prefab);

--
With registering:

UniPool.Register(TEST_Prefab);
or 
UniPool.Register(TEST_Prefab, TEST_Prefab_name);

Then do:

PoolCell cell = UniPool.Get(TEST_Prefab);
or
PoolCell cell = UniPool.Get(TEST_Prefab_name); // if you registered with a string name

--
If you want to cache one component of your choice, then use <T> versions of these methods

i.e. UniPool.Register<YourComponentType>(TEST_Prefab); and UniPool.Get<YourComponentType>(TEST_Prefab);

Note: if you register with a component type - you don't have to specify it on Get.
But if you did not register and still want to use component caching - you will need to use <T> version of Get, 
and provide your component type everytime you use Get:
UniPool.Get<YourType>(TEST_Prefab);

i.e. 
if you registered with UniPool.Register<YourComponent>(TEST_Prefab);
then you can use UniPool.Get(TEST_Prefab); and resulting PoolCell will contain cached component anyway.
Note: this is achieved by internal use of IEnumerator for generating PoolCell.
The IEnumerator holds component type you provide during registering.


Examples:
==
Dynamic  without registering:

--
Example 1. 
Prefab, no component caching (Simplest, just 1 line)
Method: UniPool.Get(GameObject prefab)

PoolCell cell = UniPool.Get(TEST_Prefab);

--
Example 2. 
Prefab with component caching (Also simple, just 1 line)
Method: PoolingManger.Get(GameObject prefab, bool componentInChildren = false)

PoolCell cell = UniPool.Get<YourComponent>(TEST_Prefab);

OR if the component is located on a child object:

PoolCell cell = UniPool.Get<YourComponent>(TEST_Prefab, true);

(1 line to Get)

==
Registering: 
Can register by:
GameObject prefab
GameObject prefab, string name - then can access either by prefab or string name
Both ways have <T> component versions

(1 line to Register)

==
Fill - you may choose to pre-fill pool with a certain amount of prefabs.
Make sure that you have either did Register, or have already used Get with this prefab,
prior to using Fill

(1 line to Register or Get, 1 line to Fill)

==
Release can be done by passing any of: PoolCell, GameObject, Transform, Component 
to UniPool.Release method

(1 line to Release)


-

Pooling Quirks:

localScale issue (especially when UGUI object is moved between parents)
It is possible to run into a situation like this:
object keeps shrinking/enlarging with every re-use. - this is a prefab editor issue:
How to fix:
Go to your prefab object you are trying to pool, and set Scale by hand.
Note: for example even if it shows 1,1,1 in inspector - in reality it's a little different,
so you need to set it to something else and then back to the scale it's meant to be, in the editor prefab view.

---

Enjoy! :)