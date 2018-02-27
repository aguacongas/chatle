import * as functions from 'firebase-functions';
import * as admin from 'firebase-admin';

admin.initializeApp(functions.config().firebase);

export const countUsers = functions.database
    .ref('/connections/{connectionId}').onWrite(event => {
        const collectionRef = event.data.ref.parent;
        const countRef = collectionRef.parent.child('connections-count');
        const timeStamp = admin.database.ServerValue.TIMESTAMP * -1;
                
        let increment;
        if (event.data.exists() && !event.data.previous.exists()) {
          increment = 1;
        } else if (!event.data.exists() && event.data.previous.exists()) {
          increment = -1;
        } else {
          return null;
        }
      
        // Return the promise from countRef.transaction() so our function
        // waits for this async event to complete before it exits.
        return countRef.transaction((current) => {
          return (current || 0) + increment;
        }).then(() => console.log(`Counter updated.`));
  });

export const connectedUsersCount = functions.database
    .ref('/connections-count').onDelete(event => {
        if (!event.data.exists()) {
            const counterRef = event.data.ref;
            const collectionRef = counterRef.parent.child('connections');

            if (!collectionRef) {
              return 0;
            }

            // Return the promise from counterRef.set() so our function
            // waits for this async event to complete before it exits.
            return collectionRef.once('value')
              .then((messagesData) => counterRef.set(messagesData.numChildren()));
          }
        return null;
});



