import { inject, Service } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Service()
export class NotificationsService {
  private snackBar = inject(MatSnackBar);

  pushGood(message: string): void {
    this.snackBar.open(message, undefined, {
      duration: 3000,
      horizontalPosition: 'center',
      verticalPosition: 'top',
      panelClass: ['lucky-snack-bar']
    });
  };

  pushBad(message: string): void {
    console.error(message);
    this.snackBar.open(message, undefined, {
      duration: 3000,
      horizontalPosition: 'center',
      verticalPosition: 'top',
      panelClass: ['sad-snack-bar']
    });
  };
}
