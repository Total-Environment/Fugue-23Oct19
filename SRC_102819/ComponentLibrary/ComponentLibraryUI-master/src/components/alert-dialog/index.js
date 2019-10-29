import React from 'react';
import Modal from 'react-modal';
import styles from './index.css';
import { Icon } from '../icon';

export const AlertDialog = (props) => <Modal className={styles.dialog} isOpen={props.shown} contentLabel="Alert">
  <Icon name="squirrel" className={styles.icon} />
  <div className={styles.content}>
    <h3 className={styles.title}>{props.title}</h3>
    <p className={styles.message}>{props.message}</p>
  </div>
  <button className={styles.button} onClick={props.onClose}>OK</button>
</Modal>;
