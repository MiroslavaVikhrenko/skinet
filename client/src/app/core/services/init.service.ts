import { inject, Injectable } from '@angular/core';
import { CartService } from './cart.service';
import { forkJoin, of } from 'rxjs';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root'
})
export class InitService {
  private cartService = inject(CartService);
  private accountService = inject(AccountService);

  init() {
    const cartId = localStorage.getItem('cart_id');
    // 'of()' to return observable | getCart() returns an observable because we used pipe() not subscribe() in the cart service
    const cart$ = cartId ? this.cartService.getCart(cartId) : of(null);

    // forkJoin allows to wait for multiple observables to complete and then emit their latest values as an array
    // when combine the results of multiple http requests and emit results only when all of them complete
    return forkJoin({
      cart: cart$,
      user: this.accountService.getUserInfo()
    })
  }
}
