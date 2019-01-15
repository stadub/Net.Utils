
### Spread of classes, used in different projects as a shared component.

### *[Service locator](../master/README_ServiceLocator.md)* service locator implementation with type mapping capabilities.
### *[Object Mapper](../master/README_Mapper.md)* -service locator implementation with type mapping capabilities.

## *[Utils.net](#utilities)* - Helper classes/extensions ##

## *Utilities* ##

### *Debugger* - simple Assert debug wrapper.
___
### *Enumerable.ForEach* - Foreach implementation for IEnumeareble<T> collection
```C#
            var resultList= new List<int>();
            var items=Enumerable.Range(0, 10).ToList();
            items.ForEach(x=>resultList.Add(x));
```
___
### *String.SplitString* - Optimized function to split string into two parts.
```C#
            var text = "1|2";
            var data = text.SplitString('|');
```
	
### *StringProperty.Trim* - Class level extension to perform more handy property trim.
```C#
	    class A{ string B {get;set;} }
	    var a = new A();
	    a.B= " text to trim ";
	    //old fashion way
	    a.b = a.B.Trim();
            //The same with Trim extension
            a.Trim( _ => _.B);
```

___
### *ExceptionHelpers*
####     GetLastWin32Exception - WinAPI wrapper function to receive latest Win32 error

####     GuardZeroHandle - Check if pointer handle is euqal zerro
___
### *EventArgs<_T_>* - generic event args class.
___
### *OperationResult<_T_>* - Result object.
```C#
    public interface IOperationResult
    {
        bool Success { get; }
        Object Value { get; }
        Exception Error { get; }
    }
    public interface IOperationResult<out T> : IOperationResult
    {
        bool Success { get; }
        T Value { get; }
        Exception Error { get; }
    }
```
___

## Utils.Wpf

VisibilityConverter - convert bool to control visibility state


### *RelayCommand* - ICommand implementation
```C#
ReloadClipboardContent = new RelayCommand(()=>Action(), () => IsAllowed);
```

### *ViewModelBase* - simple ViewModel base implementation


### *ViewModelLocatorBase* - ViewModel locator base class. 
Intended to split design time and prod ViewMdoles to different assemples.

Example usage:
```C#
 public class ViewModelLocator : ViewModelLocatorBase
{
    public ViewModelLocator(ServiceLocator container): base(container)
    {
        Register<MainWindow, MainWindowViewModel>();
    }

    public MainWindowViewModel MainPage
    {
        get { return base.Resolve<MainWindow, MainWindowViewModel>(); }
    }
	 
	public SecondWindowViewModel SecondPage
    {
        get { return base.Resolve<SecondWindow, SecondWindowViewModel>(); }
    }
}
```
