# Adhesive

Adhesive gives you a multi-direction data binding for properties of objects that stick together.  Adhesive is not intended to be used only for UI elements and can be used as method of connecting dissimilar APIs, etc.

## Requirements

- Any class who's member is used as the source for a binding, must implement `INotifyPropertyChanged`.
- Properties should call `OnPropertyChanged()` when their value has been updated.
- Properties must not call `OnPropertyChanged()` if they are set to the same value they already are (unless you don't intend on using `TwoWayBindings`).

## Usage

All bindings, in one way or another, are some combination of a `OneWayBinding` internally.

Using the example classes:
```C#
public class Employee : INotifyPropertyChanged {

        private string _firstName;
        private string _lastName;

        public string FirstName {
            get => _firstName;
            set {
                if (_firstName == value) return;

                _firstName = value;
                OnPropertyChanged();
            }
        }

        public string LastName {
            get => _lastName;
            set {
                if (_lastName == value) return;

                _lastName = value;
                OnPropertyChanged();
            }
        }

        public Employee(string firstName, string lastName) {
            this.FirstName = firstName;
            this.LastName = lastName;
        }
        
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }

    public class Nameplate {

        private string _inscribedName;

        public string InscribedName {
            get => _inscribedName;
            set {
                if (_inscribedName == value) return;

                _inscribedName = value;
                OnPropertyChanged();
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
```

#### OneWayBinding
```C#
var nameplateBinding = new Adhesive.OneWayBinding<string, string>(() => henrysNameplate.InscribedName, () => henry.FirstName);
```

Now, if `henry.FirstName` is set to any other value, `henrysNameplate.InscribedName` will immediately update to match it.


#### TwoWayBinding
```C#
var nameplateBinding = new Adhesive.TwoWayBinding<string, string>(
	() => henry.FirstName, 
	() => henrysNameplate.InscribedName, 
	 o => o.ToString().ToUpper(), 
	 o => o.ToString().ToLower(), 
	InitialBindingProcedure.ApplyRight
);
```

This will sync the values between `henry.FirstName` and `henrysNameplate.InscribedName`, but the value converters will ensure that `henry.FirstName` is always uppercase and `henryNameplate.InscribedName` is always lowercase.

#### ManyToOneBinding
```C#
var nameplateBinding = new Adhesive.ManyToOneBinding<string, string>(
	() => henrysNameplate.InscribedName,
	new List<Expression<Func<string>>>() {
		() => henry.FirstName,
		() => henry.LastName
	},
	 o => $"{henry.LastName}, {henry.FirstName}"
);
```

Updates to either `henry.FirstName` or `henry.LastName` will update `henrysNameplate.InscribedName` in the format "Lastname, Firstname."


#### OneToManyBinding
*Sample Coming Soon*

Allows for many properties to be updated when a single property is changed.


#### SyncBinding
*Sample Coming Soon*

All members are both source and target properties.  A change to any property in the binding will be applied to all other members of the binding.


## License
[MIT](https://choosealicense.com/licenses/mit/)