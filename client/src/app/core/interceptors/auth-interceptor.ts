import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  // request is immutable => need to clone
  // ensure request is with credentials
  const clonedRequest = req.clone({
    withCredentials: true
  })
  
  return next(clonedRequest);
};
