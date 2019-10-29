import React from 'react';
import sinon from 'sinon';
import sinonChai from 'sinon-chai';
import chai, {expect} from 'chai';
import { shallow } from 'enzyme';
import chaiEnzyme from 'chai-enzyme';
import { AlertDialog, __RewireAPI__ as AlertDialogRewired } from '../../../src/components/alert-dialog';

chai.use(chaiEnzyme());
chai.use(sinonChai);

describe('Alert Dialog', () => {
  describe('render', () => {
    it('should render React Modal with message', () => {
      AlertDialogRewired.__Rewire__('styles', {dialog: 'alert-dialog'});
      const wrapper = shallow(<AlertDialog message="Abdulsattar" title="Heading" shown />);
      expect(wrapper).to.have.className('alert-dialog');
      expect(wrapper).to.have.descendants('Modal');
      expect(wrapper.find('Modal')).prop('isOpen').to.be.true;
      expect(wrapper.find('p')).to.have.text('Abdulsattar');
      expect(wrapper.find('h3')).to.have.text('Heading');
    });

    it('should call onClose when Ok is clicked', () => {
      const onCloseSpy = sinon.spy();
      const wrapper = shallow(<AlertDialog message="Abdulsattar" shown onClose={onCloseSpy} />);
      wrapper.find('button').simulate('click');
      expect(onCloseSpy).to.have.been.called;
    });
  });
});
