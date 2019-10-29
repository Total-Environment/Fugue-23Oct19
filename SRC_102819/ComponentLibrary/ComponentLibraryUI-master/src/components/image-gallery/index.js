import React from 'react';
import ImageGallery from 'react-image-gallery';
import Modal from 'react-modal';
import {containerName, getSaasToken, idFor} from '../../helpers';
import styles from './index.css';
import { Icon } from '../icon';
import { alertAsync } from '../dialog';
import * as R from 'ramda';

export class ComponentImageGallery extends React.Component {
  constructor(props) {
    super(props);
  }
  render() {
    return <Modal id={idFor('imageGallery')} className={styles.dialog} isOpen={this.props.isOpen} contentLabel="Images">
      <div className={styles.header}>
        <button className={styles.closeBtn} id={idFor('close')}
          onClick={this.props.onCloseGallery}><Icon name="close" className={close} /></button>
      </div>
      <div className={styles.galleryContainer}>
        <ImageGallery
          showPlayButton={false}
          lazyLoad={true}
          showFullscreenButton={false}
          onImageError={e => {
            if (R.is(Function, this.props.onImageError)) {
              return this.props.onImageError();
            }
            return alertAsync('Error', (e && e.message) || 'Error loading image');
          }}
          items={this.props.images.filter(c => c && c.url).map(c => ({
            original: c.url+'?'+getSaasToken(containerName),
            thumbnail: c.url+'?'+getSaasToken(containerName)
          }))}
          slideInterval={2000} />
      </div>
    </Modal>
  }
}
