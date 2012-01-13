## PropertyTranslator

Linq translator for properties in queries (based on [Microsoft.Linq.Translations](https://github.com/damieng/Linq.Translations)). 

### What does it?

*PropertyTranslator* exchanges properties in linq queries before execution. This is especially useful if the underlying LINQ provider does not support some kind of operation or you want to add client-side calculations in your business logic to e.g. an EntityFramework model.

For a general introduction into the topic, have a look at [this blog post](http://damieng.com/blog/2009/06/24/client-side-properties-and-any-remote-linq-provider). *PropertyTranslator* actually is a enhancement of the presented solution in the post.

### What's the difference to Linq.Translations?

PropertyTranslator plays well together with [QueryInterceptor](https://github.com/davidfowl/QueryInterceptor) and thus can be added to every query in some kind of "data context" or general table / *ObjectSet* provider.

Furthermore it internally adds one more layer of abstraction to allow property translation depending on the ui culture of the current thread.

### Examples

#### Basic example

A POCO entity class from EntityFramework. Although in the database only a `FirstName` and a `LastName` field exists, the property `Name` can be used in queries, because right before execution of the query it is translated to `FirstName + ' ' + LastName`.

    public class Person
    {
    	private static readonly CompiledExpressionMap<Person, string> fullNameExpression = 
    	    DefaultTranslationOf<Person>.Property(p => p.FullName).Is(p => p.FirstName + " " + p.LastName);
    	    
    	public string FullName
    	{
    		get { return fullNameExpression.Evaluate(this); }
    	}
    	
    	public string FirstName { get; set; }
    	
    	public string LastName { get; set; }    	
    }

#### A more advanced example with ui culture dependent translations

The context: a database table, mapped with entity framework to POCO entity classes with two fields: `EnglishName` and `GermanName`. With the following snippet, you can use the `Name` property in linq queries which resolves to the name (either `EnglishName` or `GermanName`) depending on the current ui culture.

    public class Country
    {
    	private static readonly CompiledExpressionMap<Country, string> nameExpression = 
    	    DefaultTranslationOf<Country>.Property(c => c.Name).Is(c => c.EnglishName);
    	
    	static Country()
    	{
    	    DefaultTranslationOf<Country>.Property(c => c.Name).Is(c => c.EnglishName, 'en');
    	    DefaultTranslationOf<Country>.Property(c => c.Name).Is(c => c.GermanName, 'de');
    	}    	
    	
    	public string Name
    	{
    		get { return nameExpression.Evaluate(this); }
    	}
    	
    	public string EnglishName { get; set; }
    	
    	public string GermanName { get; set; }    	
    }

### How to enable PropertyTranslator

You can *enable* PropertyTranslator by adding the `PropertyVisitor` to your EntityFramework ObjectSets (of course it works not only with EntityFramework but with any LINQ provider):

    using QueryInterceptor;
    using PropertyTranslator;

    public class MyDataContext
    {
        ObjectContext context = new MyEfContext();
        
        public IQueryable<Person> PersonTable
        {
            get
            {
                var objectSet = context.CreateObjectSet<Person>("Persons");
                
                return objectSet.InterceptWith(new PropertyVisitor());
            }
        }
    }
