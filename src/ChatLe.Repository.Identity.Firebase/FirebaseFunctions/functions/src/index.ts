import * as functions from 'firebase-functions';
import * as admin from 'firebase-admin';

admin.initializeApp(functions.config().firebase);

export const countUsers = functions.database
    .ref('/connections/{connectionId}').onWrite(event => {
        const collectionRef = event.data.ref.parent;
        const countRef = collectionRef.parent.child('connections-count');
                
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

// const MAX_LOG_COUNT = 50;
// export const truncateMessages = functions.database.ref('/conversations/{conversationId}/messages/{messageId}').onCreate((event) => {
//   const parentRef = event.data.ref.parent;
//   return parentRef.once('value').then((snapshot) => {
//     if (snapshot.numChildren() >= MAX_LOG_COUNT) {
//       let childCount = 0;
//       const updates = {};
//       snapshot.forEach((child) => {
//         if (++childCount <= snapshot.numChildren() - MAX_LOG_COUNT) {
//           updates[child.key] = null;
//         }
//       });
//       // Update the parent. This effectively removes the extra children.
//       return parentRef.update(updates);
//     }
//     return null;
//   });
// });

export const messagePriority = functions.database.ref('/conversations/{conversationId}/messages/{messageId}').onCreate((event) => {
  const value = event.data.val();
  const timestamp = Date.now() * -1;
  console.log(`server timestamp ${timestamp}`);
  value['.priority'] = timestamp;
  return event.data.ref.child(event.data.key).update(value);
});



