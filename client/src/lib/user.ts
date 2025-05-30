import Cookies from 'js-cookie';
import { v4 as uuidv4 } from 'uuid';

export let userId: string;

export function initializeUserId() {
  const existing = Cookies.get('userId');
  if (existing) {
    userId = existing;
  } else {
    const newId = uuidv4();
    Cookies.set('userId', newId, { expires: 365 });
    userId = newId;
  }
}
