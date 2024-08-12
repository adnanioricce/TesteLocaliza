import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService, LoginResponse } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  loginForm: FormGroup;
  errorMessage: string = "";
  auth: AuthService;
  constructor(private authService: AuthService, private router: Router,private fb: FormBuilder) {
    this.auth = authService
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
  }
  private onSuccess(response:LoginResponse) {
    this.auth.setToken(response.token);
    this.router.navigate(['/home']); // Redirect to home or appropriate route
  }
  private onFailure(error:any){
    this.errorMessage = 'Invalid username or password';
  }
  onSubmit() {
    if (!this.loginForm.valid) {
      return
    }
    // Handle login logic here
    // console.log('Form Submitted!', this.loginForm.value);
    const { username, password } = this.loginForm.value;
    this.auth.login(username, password).subscribe({
      next: (response )=> {
        this.auth.setToken(response.token);
        this.router.navigate(['clientes']); // Redirect to home or appropriate route
      },
      error: this.onFailure
    });    
  }
}
// import { Component } from '@angular/core';
// import { FormBuilder, FormGroup, Validators } from '@angular/forms';
// import { AuthService } from '../services/auth.service';

// @Component({
//   selector: 'app-login',
//   templateUrl: './login.component.html',
//   styleUrls: ['./login.component.css']
// })
// export class LoginComponent {
//   loginForm: FormGroup;
//   errorMessage: string | null = null;

//   constructor(private fb: FormBuilder, private authService: AuthService) {
//     this.loginForm = this.fb.group({
//       email: ['', [Validators.required, Validators.email]],
//       password: ['', [Validators.required]]
//     });
//   }

//   onSubmit(): void {
//     if (this.loginForm.valid) {
//       const { email, password } = this.loginForm.value;
//       this.authService.login(email, password).subscribe({
//         next: (response:any) => {
//           // Handle successful login
//           console.log('Login successful', response);
//         },
//         error: (error:any) => {
//           // Handle error response
//           this.errorMessage = 'Invalid email or password';
//           console.error('Login error', error);
//         }
//       });
//     }
//   }
// }
