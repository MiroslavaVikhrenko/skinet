import { inject, Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root'
})
export class SnackbarService {
  private snackbar = inject(MatSnackBar); // for custom configuration

  // override styles in styles.scss

  error(message: string) {
    this.snackbar.open(message, 'Close', {
      duration: 5000, // 5 sec
      panelClass: ['snack-error']
    })
  }

  success(message: string) {
    this.snackbar.open(message, 'Close', {
      duration: 5000, // 5 sec
      panelClass: ['snack-success']
    })
  }
}
