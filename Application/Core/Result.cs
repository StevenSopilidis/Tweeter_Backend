namespace Application.Core
{

    //result after calling MediatR from the controllers
    public class Result<T>
    {
        //if the request was successfull
        public bool IsSuccessfull { get; set; }
        public bool IsUnautorized { get; set; }
        //the value of the returned object
        public T Value { get; set; }
        //message if any error aquired
        public string Error { get; set; }

        public static Result<T> Success(T value) => new Result<T>{Value= value, IsSuccessfull= true};
        public static Result<T> Failed(string error) => new Result<T>{Error=error, IsSuccessfull= false};
        public static Result<T> Unauthorize() => new Result<T>{IsUnautorized=true, IsSuccessfull= false};        
    }
}