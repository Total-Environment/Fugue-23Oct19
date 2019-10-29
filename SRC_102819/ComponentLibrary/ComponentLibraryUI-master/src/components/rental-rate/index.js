import React from 'react';
import styles from './index.css';
import classNames from 'classnames';

export class RentalRate extends React.Component {
  render() {
    if (this.props.rentalRates && this.props.rentalRates.length > 0) {
      return (<div>
        <div className={classNames(this.props.rowStyle, styles.row, styles.header)}>
          <div className={classNames(this.props.columnStyle, styles.column, styles.left)}>Rental Unit</div>
          <div className={classNames(this.props.columnStyle, styles.column, styles.right)}>Rental Rate</div>
        </div>
        {
          this.props.rentalRates.map(rentalRate =>
            <div className={classNames(this.props.rowStyle, styles.row)} key={rentalRate.unitOfMeasure}>
              <div className={classNames(this.props.columnStyle, styles.column, styles.left)}>{rentalRate.unitOfMeasure}</div>
              <div className={classNames(this.props.columnStyle, styles.column, styles.right)}>{rentalRate.rentalRateValue.value} {rentalRate.rentalRateValue.currency}</div>
            </div>)
        }
      </div>);
    }
    else if (this.props.rentalRates && this.props.rentalRates.message) {
      return (<h3 className={styles.errorMessage}>{this.props.rentalRates.message}</h3>);
    }
    else {
      return (<h3 className={styles.errorMessage}>{'No current Rental Rate information is found.'}</h3>);
    }
  }
}
